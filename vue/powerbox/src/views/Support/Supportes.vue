<template>
  <div>
    <Breadcrump :crumbs="crumbs" />
    <v-data-table
      :headers="headers"
      :items="supportList"
      :loading="loading"
      :server-items-length="totalSupportes"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewSupport
                v-can="'Support_Create'"
                ref="addSupportCom"
                @reloadSupportes="getSupportes"
              />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست مجموعه ها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:item.edit="{ item }">
        <v-btn
          v-can="'Support_Edit'"
          color="cyan"
          class="ma-2 white--text"
          @click="editItem(item)"
        >
          ویرایش
          <v-icon right dark> mdi-pencil </v-icon>
        </v-btn>
      </template>

      <template v-slot:item.delete="{ item }">
        <v-btn
          color="danger"
          v-can="'Support_Delete'"
          class="ma-2 red--text"
          @click="deleteItem(item.id)"
        >
          حذف
          <v-icon right dark> mdi-delete </v-icon>
        </v-btn>
      </template>

      <template v-slot:header.nameFa="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="nameFa ? 'primary' : ''">mdi-filter</v-icon>
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="nameFa"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="name = ''"
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
import AddNewSupport from "@/components/support/AddNewSupport.vue";
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
export default {
  name: "Supportes",
  components: {
    AddNewSupport,
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
          text: "پشتیبانی",
          disabled: true,
          href: "/supportes",
        },
      ],
      support: {},
      totalSupportes: 0,
      pages: 0,
      name: null,
      address: null,
      supportList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "نام", value: "userName", sortable: false },
        { text: "شماره موبایل", value: "phoneNumber", sortable: false },
        { text: "مجموعه", value: "complexName", sortable: false },
        { text: "ساعت شروع", value: "startTime", sortable: false },
        { text: "ساعت پایان", value: "endTime", sortable: false },
        { text: "", value: "edit", sortable: false, width: "10%" },
        { text: "", value: "delete", sortable: false, width: "10%" },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getSupportes();
      },
      deep: true,
    },
    nameFa: function () {
      if (this.name.length > 2 || this.name.length === 0) {
        this.options.page = 1;
        this.options.name = this.name;
        this.getSupportes();
      }
    },
  },
  mounted() {
    this.getSupportes();
  },

  methods: {
    async editItem(item) {
      this.$refs.addSupportCom.dialog = true;
      this.$refs.addSupportCom.supportId = item.id;
    },

    next(page) {
      this.options.page = page;
      this.getSupportes();
    },
    handler(event) {
      this.options = event;
    },
    async getSupportes() {
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
        .get("/support/supports?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.supportList = data.result;
          this.totalSupportes = data.totalItems;
          this.pages = data.pageCount;
        })
        .catch((error) => {
          alert(error);
        })
        .finally(() => {
          this.loading = false;
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
            .delete(`/support/${id}`)
            .then(() => {
              Vue.swal("", "پشتیبان با موفقیت حذف گردید", "success");
              this.getSupportes();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },
  },
};
</script>
