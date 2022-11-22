<template>
  <div>
    <Breadcrump :crumbs="crumbs" />
    <v-data-table
      :headers="headers"
      :items="reportList"
      :loading="loading"
      :server-items-length="totalReportes"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
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

           <v-btn
          color="error"
          class="ma-2 white--text"
          v-can="'TechnicalReport_Delete'"
          @click="deleteItem(item.id)"
        >
          حذف
          <v-icon right dark> mdi-delete </v-icon>
        </v-btn>
      </template>
      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewReport
                v-can="'TechnicalReport_Create'"
                ref="addReportCom"
                @reloadReportes="getReportes"
              />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>گزارشات</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:item.shortContent="{ item }">
        <v-text>
          {{ item.shortContent }}
          <v-btn  text right color="cyan" @click="getContent(item.id)"> مشاهده </v-btn>
        </v-text>
      </template>
      <template v-slot:item.file="{ item }">
        <v-btn
          v-if="item.file"
          color="cyan"
          class="ma-2 white--text"
          @click="getFile(item)"
        >
          فایل ضمیه
          <v-icon right dark> mdi-pencil </v-icon>
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
import AddNewReport from "@/components/report/AddNewReport.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";

export default {
  name: "Reportes",
  components: {
    AddNewReport,
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
          text: "گزارشات",
          disabled: true,
          href: "/reportes",
        },
      ],
      report: {},
      totalReportes: 0,
      pages: 0,
      name: null,
      reportList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "متن", value: "shortContent", sortable: false },
        { text: "ضمیمه", value: "file", sortable: false },
        { text: "تاریخ", value: "date", sortable: false },
        { text: "دستگاه", value: "deviceName", sortable: false },
        { text: "مجموعه", value: "complexName", sortable: false },
        { text: "", value: "actions", sortable: false, width: "20%" },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getReportes();
      },
      deep: true,
    },
    nameFa: function () {
      if (this.name.length > 2 || this.name.length === 0) {
        this.options.page = 1;
        this.options.name = this.name;
        this.getReportes();
      }
    },
  },
  mounted() {
    this.getReportes();
  },

  methods: {

     async deleteItem(id) {
      Vue.swal({
        title: "ایا برای حذف این گزارش مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,حذف شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request.delete(`report/${id}`).then(() => {
            this.uploadLoading = false;
            Vue.swal("", "گزارش  با موفقیت حذف گردید", "success");
            this.getComplexes();
          });
        }
      });
    },
    getContent(id) {
      var list =this.reportList.filter(c=>c.id == id);
      alert(list[0].content);
    },
    getFile(item) {
      request.get(`${item.file}`, { responseType: "blob" }).then((response) => {
        debugger;
        var type = response.headers["content-type"].split("/")[1];
        var fileURL = window.URL.createObjectURL(new Blob([response.data]));
        var fileLink = document.createElement("a");
        fileLink.href = fileURL;
        alert(type);
        fileLink.setAttribute("download", "file." + type);
        document.body.appendChild(fileLink);

        fileLink.click();
      });
    },
    async editItem(item) {
      this.$refs.addReportCom.dialog = true;
      this.$refs.addReportCom.reportId = item.id;
    },

    next(page) {
      this.options.page = page;
      this.getReportes();
    },
    handler(event) {
      this.options = event;
    },
    async getReportes() {
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
        .get("/report/reports?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.reportList = data.result;
          this.totalReportes = data.totalItems;
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
