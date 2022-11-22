<template>
  <div>
    <Breadcrump :crumbs="crumbs" />
    <v-data-table
      :headers="headers"
      :items="notificationList"
      :loading="loading"
      item-key="id"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewNotification
                v-can="'Notif_Content_Create'"
                ref="addNotificationCom"
                @reloadNotification="getNotificationes"
              />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست پیامها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:item.actions="{ item }">
        <v-btn
          v-can="'Notif_Content_Edit'"
          color="cyan"
          class="ma-2 white--text"
          @click="editItem(item)"
        >
          ویرایش
          <v-icon right dark> mdi-pencil </v-icon>
        </v-btn>
      </template>
    </v-data-table>
  </div>
</template>
<script>
import request from "@/utils/request";
import AddNewNotification from "@/components/notification/AddNewNotification.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
export default {
  name: "Notificationes",
  components: {
    AddNewNotification,
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
          href: "/notificationes",
        },
      ],
      notification: {},
      totalNotificationes: 0,
      pages: 0,
      notificationList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "عنوان", value: "title", sortable: false },
        { text: "متن", value: "content", sortable: false },
        { text: "تاریخ آخرین ویرایش", value: "updateAt", sortable: false },
        { text: "", value: "actions", sortable: false, width: "10%" },
      ],
    };
  },
  mounted() {
    this.getNotificationes();
  },

  methods: {
    async editItem(item) {
      this.$refs.addNotificationCom.dialog = true;
      this.$refs.addNotificationCom.notificationId = item.id;
    },

    next(page) {
      this.options.page = page;
      this.getNotificationes();
    },

    async getNotificationes() {
      this.loading = true;
      await request
        .get("/notification/notifications")
        .then((response) => {
          var data = response.data.result;
          this.notificationList = data.result;
          this.totalNotificationes = data.totalItems;
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
