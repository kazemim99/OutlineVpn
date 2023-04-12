<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-data-table
      :headers="headers"
      :items="orders"
      :loading="loading"
      :server-items-length="totalOrders"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.statuses="{ item }">
        <v-select
          @change="changeState(item.id, $event)"
          :items="item.statuses"
          item-value="id"
          item-text="text"
        ></v-select>
      </template>

      <template v-slot:header.keyUserName="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
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
      stateId: 0,
      totalOrders: 0,
      switchLoading: null,
      pages: 0,
      serverid: 0,
      isActive: null,
      title: null,
      orders: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },
      headers: [
        { text: "کاربر", value: "creator", sortable: false },
        { text: "تاریخ ایجاد", value: "createdAt", sortable: true },
        { text: "شماره کارت", value: "cardNumber", sortable: false },
        { text: "مبلغ", value: "amount", sortable: false },
        { text: "کلید", value: "keyUserName", sortable: false },
        { text: "وضعیت", value: "status", sortable: true },
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
    if (this.$store.state.userDetails.isAdmin) {
      this.headers.push({
        text: "وضعیت",
        value: "statuses",
        sortable: false,
        widh: "150",
      });
    }
  },
  created() {
    this.getOrders();
  },

  methods: {
    async changeState(id, stateId) {
      debugger;
      this.switchLoading = "warning";
      await request
        .put(`/order/change-state/${id}/${stateId}`)
        .then(() => {
          this.getOrders();
        })
        .catch((error) => {
          alert(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },
    async editItem(item) {
      this.$refs.addOrderCom.dialog = true;
      this.$refs.addOrderCom.id = item.id;
    },
    deleteItem(id) {
      Vue.swal({
        title: "ایا مطمئن  هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,حذف شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request
            .delete(`/Order/${id}`)
            .then(() => {
              Vue.swal("", "کلید با موفقیت حذف گردید", "success");
              this.getOrders();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
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

    async getOrders() {
      const { sortBy, sortDesc, page, itemsPerPage } = this.options;
      this.loading = true;
      // this.options.sortDesc = true;
      // this.options.sortBy = "createdAt";
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
          var data = response.data.result;
          this.orders = data.result;
          this.totalOrders = data.totalItems;
          this.pages = data.pageCount;
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
