<template>
  <div>
    <v-row class="mb-4">
      <Breadcrump class="mb-2" :crumbs="crumbs" />
      <v-spacer></v-spacer>

      <v-btn
        @click="$router.go(-1)"
        class="mx-10 mt-5"
        fab
        small
        dark
        color="indigo"
      >
        <v-icon dark> mdi-arrow-left </v-icon>
      </v-btn>
    </v-row>
    <v-data-table
      :headers="headers"
      :items="userList"
      :loading="loading"
      :server-items-length="totalUsers"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.userState="{ item }">
        <v-switch
          v-model="item.userState"
          v-if="!complexId"
          flat
          @change="changeUserState(item)"
          :label="`${item.userState ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>

      <template
        v-if="!complexId"
        v-can="'Member_Edit'"
        v-slot:item.actions="{ item }"
      >
        <v-icon medium class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
      </template>

      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewUser
                v-if="!complexId"
                v-can="'Member_Create'"
                :userId="selelectedUserId"
                :selectedComplexId="complexId"
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

      <template v-slot:header.firstName="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="firstName ? 'primary' : ''"
                >mdi-filter</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="firstName"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="firstName = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>

      <template v-slot:header.lastName="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="lastName ? 'primary' : ''"
                >mdi-filter</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="lastName"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="lastName = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>

      <template v-slot:header.mobile="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="mobile ? 'primary' : ''">mdi-filter</v-icon>
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="mobile"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="mobile = ''"
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
      v-can="'Member_List'"
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
          text: "مجموعه ها",
          disabled: false,
          href: "/complexes",
        },
        {
          text: `مجموعه  ${this.$store.state.selectedComplexName}`,
          disabled: false,
          href: `/complex-units/${this.$route.params.id}`,
        },
        {
          text: "باشگاه مشتریان",
          disabled: true,
        },
      ],
      user: {},
      selelectedUserId: null,
      totalUsers: 0,
      isUnit : null,
      switchLoading: null,
      pages: 0,
      complexId: null,
      userState: null,
      firstName: null,
      lastName: null,
      mobile: null,
      userList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "نام", value: "firstName", sortable: true },
        { text: "نام خانوادگی", value: "lastName", sortable: true },
        { text: "نام کاربری", value: "mobile", sortable: false },
        { text: "وضعیت", value: "userState", sortable: true },
        { text: "مجموعه ها", value: "complexes", sortable: false },
        { text: "", value: "actions", sortable: false },
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
    firstName: function () {
      if (this.firstName.length > 2 || this.firstName.length === 0)
        this.options.page = 1;
      this.options.firstName = this.firstName;

      this.getUsers();
    },
    lastName: function () {
      if (this.lastName.length > 2 || this.lastName.length === 0)
        this.options.page = 1;
      this.options.lastName = this.lastName;

      this.getUsers();
    },
    mobile: function () {
      if (this.mobile.length > 2 || this.mobile.length === 0)
        this.options.page = 1;
      this.options.mobile = this.mobile;

      this.getUsers();
    },
  },
  mounted() {
    this.complexId = this.$route.params.id;
    this.getUsers();
  },

  methods: {
    async changeUserState(item) {
      this.switchLoading = "warning";
      await request
        .put(`/user/change-state/${item.id}`)
        .then(() => {
          console.log(item.userState);
        })
        .catch((error) => {
          alert(error);
          this.userState = !this.userState;
        })
        .finally(() => {
          this.loading = false;
        });
    },
    async editItem(item) {
      this.selelectedUserId = item.id;
      this.$refs.addUserCom.dialog = true;
      this.$refs.addUserCom.userId = item.id;
    },

    next(page) {
      this.options.page = page;
      this.getUsers();
    },
    handler(event) {
      this.options = event;
    },
    GetSelectedState(state) {
      this.userState = state;
    },

    async getUsers() {
      const { sortBy, sortDesc, page, itemsPerPage } = this.options;
      this.loading = true;
      this.options.complexId = this.complexId;
      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");

      this.loading = true;
      await request
        .get("/complex/users?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
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
