<template>
  <div>
    <Breadcrump :crumbs="crumbs" />
    <v-data-table
      :headers="headers"
      :items="deviceNotificationList"
      :server-items-length="totalDeviceNotifications"
      :options.sync="options"
      :loading="loading"
      item-key="id"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>
          <v-toolbar-title>لیست پیامها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:item.actions="{ item }">
        <v-btn color="cyan" class="ma-2 white--text" @click="deleteItem(item)">
          حذف
          <v-icon right dark> mdi-trash </v-icon>
        </v-btn>
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
export default {
  name: "DeviceNotifications",
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
          text: "پیغامها",
          disabled: true,
          href: "/deviceDeviceNotificationes",
        },
      ],
      totalDeviceNotifications: 0,
      pages: 0,
      deviceNotificationList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },
      headers: [
        { text: "تاریخ", value: "createAt", sortable: false },
        { text: "محتوا", value: "content", sortable: false },
        { text: "مجموعه", value: "complexName", sortable: false },
        { text: "دستگاه", value: "deviceName", sortable: false },
        { text: "لاکر", value: "lockerNo", sortable: false },
        { text: "", value: "actions", sortable: false, width: "10%" },
      ],
    };
  },
  mounted() {
    this.getDeviceNotificationes();
  },

  watch: {
    options: {
      handler() {
        this.getDeviceNotificationes();
      },
      deep: true,
    },
  },
  methods: {
    next(page) {
      this.options.page = page;
      this.getDeviceNotificationes();
    },
    async getDeviceNotificationes() {
      const { sortDesc, sortBy, page, itemsPerPage } = this.options;
      this.loading = true;

      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");
      await request
        .get("/notification/device-notifications?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.deviceNotificationList = data.result;
          this.totalDeviceNotifications = data.totalItems;
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
