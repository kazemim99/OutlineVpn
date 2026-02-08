<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-row>
      <v-col cols="6" md="6" sm="12">
        <v-select
          v-if="this.$store.state.userDetails.isAdmin"
          v-model="input.userId"
          :items="users"
          item-value="id"
          item-text="firstName"
          label="کاربر"
          @change="getOrders()"
          solo
        ></v-select>
      </v-col>
      <v-col cols="6" sm="12" md="4">
        <v-select
          v-model="input.durationId"
          :items="durations"
          item-value="id"
          item-text="title"
          label="اعتبار"
          solo
        ></v-select>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="6" sm="12" md="4">
        <label>از :</label>
        <date-picker v-model="input.from" simple />
      </v-col>

      <v-col cols="6" sm="12" md="4">
        <label>تا :</label>
        <date-picker v-model="input.to" simple />
      </v-col>
    </v-row>

    <label>یک ماهه : {{ oneMonthCount }}</label>
    <br />
    <label>دو ماهه : {{ twoMonthCount }}</label>
    <br />
    <label>سه ماهه : {{ threeMonthCount }}</label>
    <br />
    <label>نامشخص : {{ unknownCount }}</label>
    <br />
    <v-btn color="primary" dark @click="submit()"> اعمال </v-btn>

    <br />
    <br />
    <v-data-table
      :headers="headers"
      :items="orders"
      :loading="loading"
      :server-items-length="totalOrders"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template #item.statuses="{ item }">
        <v-select
          @change="changeState(item.id, $event)"
          :items="item.statuses"
          item-value="id"
          item-text="text"
        ></v-select>
      </template>

      <template #header.keyUserName="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template #activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="keyUserName ? 'primary' : ''"
                >mdi-filter</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="keyUserName"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="keyUserName = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>
    </v-data-table>
    <v-pagination
      v-model="options.page"
      @input="next"
      :length="this.pages"
      :total-visible="7"
    ></v-pagination>
  </div>
</template>
<script>
import request from "@/utils/request";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";

export default {
  name: "Orders",
  computed: {},
  components: {
    Breadcrump,
  },
  data() {
    return {
      crumbs: [
        {
          text: "خانه",
          disabled: false,
          href: "/",
        },
        {
          text: "کلیدها",
          disabled: true,
        },
      ],
      durations: [
        { id: 30, title: "1 ماهه" },
        { id: 60, title: "2 ماهه" },
        { id: 90, title: "3 ماهه" },
        { id: 0, title: "نام مشخص" },
      ],

      input: {
        userId: null,
        from: null,
        to: null,
        durationId: null,
      },
      stateId: 0,
      oneMonthCount: 0,
      twoMonthCount: 0,
      unknownCount: 0,
      threeMonthCount: 0,
      switchLoading: null,
      pages: 0,
      serverid: 0,
      isActive: null,
      users: [],
      title: null,
      orders: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },
      headers: [
        { text: "کاربر", value: "creator", sortable: false },
        { text: "تاریخ تمدید یا ایجاد", value: "createdAt", sortable: true },
        { text: "تاریخ انتقضا", value: "expireDate", sortable: true },
        { text: "کلید", value: "keyUserName", sortable: false },
        { text: "دوره", value: "duration", sortable: false },
      ],
    };
  },

  watch: {
    options: {
      handler() {
        this.getOrders();
      },
      deep: true,
    },
    title: function () {
      if (this.title.length > 2 || this.title.length === 0)
        this.options.page = 1;
      this.options.title = this.title;

      this.getOrders();
    },
  },
  mounted() {
    // if (this.$store.state.userDetails.isAdmin) {
    //   this.headers.push({
    //     text: "وضعیت",
    //     value: "statuses",
    //     sortable: false,
    //     widh: "150",
    //   });
    // }
  },

  created() {
    this.getOrders();
    this.getUsers();
  },

  methods: {
    async getUsers() {
      await request
        .get("/user/users")
        .then((response) => {
          const data = response.data.result;
          this.users = data.result;
        })
        .catch((error) => {
          alert(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },

    next(page) {
      this.options.page = page;
      this.Orders();
    },
    handler(event) {
      this.options = event;
    },
    GetSelectedState(state) {
      this.state = state;
    },
    async submit() {
      this.getOrders();
    },
    async getOrders() {
      const { sortBy, sortDesc, page, itemsPerPage } = this.options;
      this.loading = true;
      // this.options.sortDesc = true;
      // this.options.sortBy = "createdAt";
      this.options.userId = this.input.userId;
      this.options.from = this.input.from;
      this.options.to = this.input.to;
      this.options.durationId = this.input.durationId;
      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");

      this.loading = true;
      await request
        .get("/order/filter?" + filterQuery)
        .then((response) => {
          const data = response.data.result;
          this.orders = data.result;
          this.totalOrders = data.totalItems;
          this.pages = data.pageCount;

          request.get("/order/orderCount?" + filterQuery).then((response) => {
            const data = response.data.result;
            this.oneMonthCount = data.oneMonthCount;
            this.twoMonthCount = data.twoMonthCount;
            this.threeMonthCount = data.threeMonthCount;
            this.unknownCount = data.unknownCount;
          });
        })
        .catch((error) => {
          alert(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },
  },
};
</script>
