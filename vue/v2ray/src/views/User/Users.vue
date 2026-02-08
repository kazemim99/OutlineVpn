<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-data-table
      :headers="headers"
      :items="userList"
      :loading="loading"
      :server-items-length="totalUsers"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template #item.enable="{ item }">
        <v-switch
          v-model="item.enable"
          flat
          @change="changeUserState(item)"
          :label="`${item.enable ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>

      <template #item.edit="{ item }">
        <v-icon
          v-can="'Member_Edit'"
          medium
          class="mr-2"
          @click="editItem(item)"
          >mdi-pencil</v-icon
        >
      </template>

      <template #item.delete="{ item }">
        <v-icon
          v-can="'Member_Delete'"
          medium
          class="mr-2"
          @click="deleteItem(item.id)"
          >mdi-delete</v-icon
        >
      </template>

      <template #top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewUser
                v-can="'Member_Create'"
                ref="addUserCom"
                @reloadUsers="getUsers"
              />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست کاربران</v-toolbar-title>
        </v-toolbar>
      </template>
      <template #header.email="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template #activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="email ? 'primary' : ''">mdi-filter</v-icon>
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="email"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="email = ''"
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
import AddNewUser from "@/components/user/AddNewUser.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";

export default {
  name: "Users",
  components: {
    // UserStates
    AddNewUser,
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
          text: "کاربران",
          disabled: true,
        },
      ],
      user: {},
      totalUsers: 0,
      switchLoading: null,
      pages: 0,
      enable: null,
      firstName: null,
      lastName: null,
      email: null,
      userList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "نام", value: "firstName", sortable: true },
        { text: "نام خانوادگی", value: "lastName", sortable: true },
        { text: "نام کاربری", value: "mobile", sortable: false },
        { text: "وضعیت", value: "enable", sortable: true },
        { text: "", value: "edit", sortable: false },
        { text: "", value: "delete", sortable: false },
        { text: "", value: "sendaccesskey", sortable: false },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getUsers();
      },
      deep: true,
    },
    email: function () {
      if (this.email.length > 2 || this.email.length === 0)
        this.options.page = 1;
      this.options.email = this.email;

      this.getUsers();
    },
  },
  mounted() {
    this.getUsers();
  },

  methods: {
    async changeUserState(item) {
      console.log(item);
      this.switchLoading = "warning";
      await request
        .put(`/user/change-state/${item.id}`)
        .then(() => {
          console.log(item.enable);
        })
        .catch((error) => {
          alert(error);
          this.enable = !this.enable;
        })
        .finally(() => {
          this.loading = false;
        });
    },
    async editItem(item) {
      this.$refs.addUserCom.dialog = true;
      this.$refs.addUserCom.userId = item.id;
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
            .delete(`/user/${id}`)
            .then(() => {
              Vue.swal("", "کاربر با موفقیت حذف گردید", "success");
              this.getUsers();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

    next(page) {
      this.options.page = page;
      this.getUsers();
    },
    handler(event) {
      this.options = event;
    },
    GetSelectedState(state) {
      this.enable = state;
    },

    async getUsers() {
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
        .get("/user/users?" + filterQuery)
        .then((response) => {
          const data = response.data.result;
          this.userList = data.result;
          this.totalUsers = data.totalItems;
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
