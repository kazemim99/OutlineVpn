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
      v-can="'Role_List'"
      :headers="headers"
      :items="complexRoleList"
      :loading="loading"
      :server-items-length="totalComplexRole"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.actions="{ item }">
        <v-btn color="cyan" class="ma-2 white--text"  v-can="'Role_Edit'" @click="editItem(item)">
          ویرایش
          <v-icon right dark> mdi-pencil </v-icon>
        </v-btn>
      </template>
      <template v-slot:item.delete="{ item }"  >
        <v-icon color="red" v-can="'Role_Delete'" right dark @click="deleteComplexRole(item.id)">
          mdi-delete
        </v-icon>
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

      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewComplexRole
              v-can="'Role_Add'"
                ref="addComplexRoleCom"
                @reloadRoles="getComplexRole"
              />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>نقشهای مجموعه</v-toolbar-title>
        </v-toolbar>
      </template>
    </v-data-table>
    <v-pagination
      v-can="'Role_List'"
      v-model="options.page"
      @input="next"
      :length="this.pages"
      :total-visible="7"
    ></v-pagination>
  </div>
</template>
<script>
import request from "@/utils/request";
import AddNewComplexRole from "@/components/complex/AddNewComplexRole.vue";
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";

export default {
  name: "ComplexRole",

  components: {
    AddNewComplexRole,
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
          text: "نقش ها",
          disabled: true,
        },
      ],
      pages: 0,
      totalComplexRole:null,
      complexRoleList:[],
      loading: true,
      title:null,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "نام نمایشی", value: "appearanceTitle", sortable: true },
        { text: "نام", value: "title", sortable: true },
        { text: "", value: "actions", sortable: false, width: "10%" },
        { text: "", value: "delete", sortable: false, width: "10%" },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getComplexRole();
      },
      deep: true,
    },
    title: function () {
      if (this.title.length > 2 || this.title.length === 0) {
        this.options.page = 1;
        this.options.title = this.title;
        this.getComplexRole();
      }
    },
  },
  mounted() {
    this.complexId = this.$route.params.id;
    this.getComplexRole();
  },

  methods: {
    deleteComplexRole(roleId) {
      Vue.swal({
        title: "برای حذف این نقش مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,حذف شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request.delete(`/complexRole/${roleId}`).then(() => {
            Vue.swal("", "نقش  با موفقیت حذف گردید", "success");
            this.getComplexRole();
          });
        }
      });
    },

    async editItem(item) {
      this.$refs.addComplexRoleCom.dialog = true;
      this.$refs.addComplexRoleCom.complexRoleId = item.id;
    },

    next(page) {
      this.options.page = page;
      this.getComplexRole();
    },
    handler(event) {
      this.options = event;
    },
    async getComplexRole() {
      const { sortDesc, sortBy, page, itemsPerPage } = this.options;
      this.loading = true;

      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");

      this.loading = true;
      await request
        .get(`/complexRole/roles?` + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.complexRoleList = data.result;
          this.totalComplexRole = data.totalItems;
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
