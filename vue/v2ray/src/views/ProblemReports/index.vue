<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-data-table
      :headers="headers"
      :items="problemReportList"
      :loading="loading"
      :server-items-length="totalProblemReports"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template
        v-if="this.$store.state.userDetails.isAdmin"
        #item.edit="{ item }"
      >
        <v-icon
          v-can="'Member_Edit'"
          medium
          class="mr-2"
          @click="openAnswer(item.id)"
          >mdi-pencil</v-icon
        >
      </template>

      <template #item.show="{ item }">
        <v-btn
          v-if="item.answer"
          medium
          class="mr-2"
          @click="getAnswer(item.answer)"
          >مشاهده پاسخ</v-btn
        >
      </template>

      <template #top>
        <v-toolbar flat>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست پیامها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template #heade.email="{ header }">
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
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";

export default {
  name: "ProblemReports",
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
          text: "گزارش مشکل",
          disabled: true,
        },
      ],
      problemReport: {},
      totalProblemReports: 0,
      switchLoading: null,
      pages: 0,
      enable: null,
      firstName: null,
      lastName: null,
      email: null,
      model: {
        answer: "",
      },
      problemReportList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "نام کاربری", value: "userName", sortable: true },
        { text: "اپراتور", value: "operator", sortable: true },
        { text: "سیستم عامل", value: "os", sortable: false },
        { text: "وضعیت", value: "state", sortable: true },
        { text: "بازگشت وجه", value: "returnMoney", sortable: false },
        { text: "", value: "edit", sortable: false },
        { text: "", value: "show", sortable: false },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getProblemReports();
      },
      deep: true,
    },
    email: function () {
      if (this.email.length > 2 || this.email.length === 0)
        this.options.page = 1;
      this.options.email = this.email;

      this.getProblemReports();
    },
  },
  mounted() {
    this.getProblemReports();
  },

  methods: {
    async openAnswer(id) {
      const answer = prompt("پاسخ", "");
      if (answer != null) {
        this.model.answer = answer;
        this.sendAnwer(id);
      }
    },
    async sendAnwer(id) {
      request
        .put(`/ProblemReport/sendAnswer/${id}`, this.model)
        .then(() => {
          Vue.swal("", "پاسخ با موفقیت ارسال شد", "success");
          this.getProblemReports();
        })
        .finally(() => {
          this.uploadLoading = false;
        });
    },
    async editItem(item) {
      this.$refs.addProblemReportCom.dialog = true;
      this.$refs.addProblemReportCom.problemReportId = item.id;
    },
    getAnswer(answer) {
      Vue.swal("", `${answer}`);
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
            .delete(`/problemReport/${id}`)
            .then(() => {
              Vue.swal("", "کاربر با موفقیت حذف گردید", "success");
              this.getProblemReports();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

    next(page) {
      this.options.page = page;
      this.getProblemReports();
    },
    handler(event) {
      this.options = event;
    },
    GetSelectedState(state) {
      this.enable = state;
    },

    async getProblemReports() {
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
        .get("/problemReport/?" + filterQuery)
        .then((response) => {
          const data = response.data.result;
          this.problemReportList = data.result;
          this.totalProblemReports = data.totalItems;
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
