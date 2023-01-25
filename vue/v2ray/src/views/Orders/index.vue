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
      <template v-slot:item.isActive="{ item }">
        <v-switch
          v-model="item.isActive"
          flat
          @change="changeState(item)"
          :label="`${item.isActive ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>

      <template v-slot:item.edit="{ item }">
        <v-icon
          v-can="'Member_Edit'"
          medium
          class="mr-2"
          @click="editItem(item)"
          >mdi-pencil</v-icon
        >
      </template>

      <template v-slot:item.delete="{ item }">
        <v-icon
          v-can="'Member_Delete'"
          medium
          class="mr-2"
          @click="deleteItem(item.id)"
          >mdi-delete</v-icon
        >
      </template>

     
      <template v-slot:header.title="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="title ? 'primary' : ''">mdi-filter</v-icon>
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="title"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="title = ''"
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
        { text: "ایمیل", value: "email", sortable: true },
        { text: "آی پی", value: "iP", sortable: false },
        { text: "تاریخ ایجاد", value: "createDate", sortable: false },
        { text: "تاریخ انقضا", value: "expireDate", sortable: false },
        { text: "", value: "edit", sortable: false },
        { text: "", value: "delete", sortable: false },
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
  created() {
    this.getOrders();
  },

  methods: {
    async changeState(item) {
      this.switchLoading = "warning";
      await request
        .put(`/order/change-state/${item.id}`)
        .then(() => {
          console.log(item.isActive);
        })
        .catch((error) => {
          alert(error);
          this.isActive = !this.isActive;
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
