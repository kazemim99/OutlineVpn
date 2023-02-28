<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-data-table
      :headers="headers"
      :items="sshKeys"
      :loading="loading"
      :server-items-length="totalSSHKeys"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.enable="{ item }">
        <v-switch
          v-model="item.enable"
          flat
          @change="enableKey(item.id)"
          :label="`${item.enable ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>

      <template v-slot:item.delete="{ item }">
        <v-icon medium class="mr-2" @click="deleteItem(item.id)"
          >mdi-delete</v-icon
        >
      </template>

      <template v-slot:item.edit="{ item }">
        <v-icon medium class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
      </template>

      <template v-slot:item.copy="{ item }">
        <v-row>
          <v-icon
            medium
            class="mr-2"
            @click="
              copyToClipBoard(
                `username: ${item.userName} \n password: ${item.password}`
              )
            "
          >
            mdi-content-copy</v-icon
          >
        </v-row>
      </template>

      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="12">
            <template right>
              <v-row>
                <v-col class="d-flex" cols="3" sm="6">
                  <AddNew
                    v-can="'Member_Create'"
                    ref="addSSHKeyCom"
                    @reloadSSHKeys="getSSHKeys"
                  />
                </v-col>
                <v-col class="d-flex" cols="3" sm="6">
                  <v-switch
                    v-model="expired"
                    flat
                    @change="expireKey()"
                    :label="`${expired ? 'فعال' : 'غیر فعال'}`"
                  ></v-switch>
                </v-col>
              </v-row>
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست سرور ها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:header.userName="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="userName ? 'primary' : ''"
                >mdi-filter</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="userName"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="userName = ''"
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
import AddNew from "@/components/SSHKeys/AddNew.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";

export default {
  name: "SSHKeys",
  components: {
    AddNew,
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
      totalSSHKeys: 0,
      switchLoading: null,
      pages: 0,
      expired: false,
      serverid: 0,
      isActive: null,
      userName: null,
      sshKeys: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },
      headers: [
        { text: "نام کاربری", value: "userName", sortable: true },
        { text: "رمز عبور", value: "password", sortable: false },
        { text: "سرور", value: "serverName", sortable: false },
        { text: "تاریخ انقضا", value: "expireDate", sortable: true },
        { text: "وضعیت", value: "enable", sortable: true },
        { text: "", value: "edit", sortable: false },
        { text: "", value: "delete", sortable: false },
        { text: "", value: "copy", sortable: false },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getSSHKeys();
      },
      deep: true,
    },

    userName: function () {
      if (this.userName.length > 2 || this.userName.length === 0)
        this.options.page = 1;
      this.options.userName = this.userName;

      this.getSSHKeys();
    },
  },
  created() {
    this.getSSHKeys();
  },

  methods: {
    expireKey() {
      this.options.page = 1;
      this.options.expired = this.expired;
      this.options.sortBy = "enable";
      this.getSSHKeys();
    },
    async editItem(item) {
      this.$refs.addSSHKeyCom.dialog = true;
      this.$refs.addSSHKeyCom.id = item.id;
    },

    copyToClipBoard(textToCopy) {
      navigator.clipboard
        .writeText(textToCopy)
        .then(() => {
          console.log(textToCopy);
        })
        .catch(() => {
          alert("خطا در کپی");
        });
    },
    async enableKey(id) {
      request.put(`/SSHKey/change-state/${id}`).then(() => {
        this.getSSHKeys();
      });
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
            .delete(`/SSHKey/${id}`)
            .then(() => {
              Vue.swal("", "کلید با موفقیت حذف گردید", "success");
              this.getSSHKeys();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

    next(page) {
      this.options.page = page;
      this.SSHKeys();
    },
    handler(event) {
      this.options = event;
    },
    GetSelectedState(state) {
      this.state = state;
    },

    async getSSHKeys() {
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
        .get("/sshkey/filter?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.sshKeys = data.result;
          this.totalSSHKeys = data.totalItems;
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
