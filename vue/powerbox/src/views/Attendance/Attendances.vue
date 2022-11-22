<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-row>
      <v-col cols="3">
        <v-text-field
          v-model="options.userFullName"
          class="pa-4"
          type="text"
          label="نام"
        ></v-text-field>
      </v-col>
      <v-col cols="3">
        <v-text-field
          v-model="options.code"
          class="pa-4"
          type="text"
          label="کد"
        ></v-text-field>
      </v-col>
      <v-col cols="3">
        <v-select
          v-model="options.isEnter"
          :items="EnterTypes"
          item-value="id"
          item-text="text"
          label="نوع "
        ></v-select>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="3">
        <v-menu
          v-model="menu1"
          :close-on-content-click="false"
          :nudge-right="40"
          transition="scale-transition"
          offset-y
          min-width="auto"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              clearable
              @click:clear="clearFrom()"
              v-model="formattedDate"
              label="از"
              prepend-icon="mdi-calendar"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            :first-day-of-week="0"
            locale="fa-ir"
            v-model="options.from"
            @input="menu1 = false"
          ></v-date-picker>
        </v-menu>
      </v-col>

      <v-col cols="3">
        <v-menu
          v-model="menu2"
          :close-on-content-click="false"
          :nudge-right="40"
          transition="scale-transition"
          offset-y
          min-width="auto"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              clearable
              @click:clear="clearTo()"
              v-model="formattedDate1"
              label="تا"
              prepend-icon="mdi-calendar"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            :first-day-of-week="0"
            locale="fa-ir"
            v-model="options.to"
            @input="menu2 = false"
          ></v-date-picker>
        </v-menu>
      </v-col>
    </v-row>
    <v-row>
      <v-btn
        @click="getAttendances()"
        class="mx-10 mt-5 mb-10"
        small
        dark
        color="indigo"
      >
        اعمال فیلتر
      </v-btn>
      <v-btn
        :loading="loading"
        class="mx-10 mt-5 mb-10"
        small
                color="red"

        @click="submit()"
        >فایل اکسل</v-btn
      >
    </v-row>

    <v-data-table
      :headers="headers"
      :items="attendanceList"
      :loading="loading"
      :server-items-length="totalAttendances"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.enterImage="{ item }">
        <img
          @click="getModal(item.enterImage)"
          ref="el"
          :src="item.enterImage"
          width="50"
          height="50"
        />
      </template>

      <template v-slot:item.entered="{ item }">
        <v-chip class="ma-2" :color="`${!item.entered ? 'red' : 'green'}`">
          {{ item.entered ? "ورود" : "خروج" }}
        </v-chip>
      </template>
      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <!-- <template right>
              <AttnedanceReport
                v-can="'Support_Create'"
                ref="addSupportCom"
                @reloadSupportes="getAttendances"
              />
            </template> -->
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>حضور و غیاب</v-toolbar-title>
        </v-toolbar>
      </template>
    </v-data-table>
    <v-pagination
      v-model="options.page"
      @input="next"
      :length="this.pages"
      :total-visible="7"
    ></v-pagination>
    <rotate ref="modal"></rotate>
  </div>
</template>
<script>
import request from "@/utils/request";
import AttnedanceReport from "@/components/Attnedance/AttnedanceReport.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import rotate from "@/components/common/Rotate.vue";
export default {
  name: "Attendances",
  components: {
    Breadcrump,
    rotate,
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
          text: "حضور و غیاب",
          disabled: true,
        },
      ],
      EnterTypes: [
        {
          text: "نوع",
          id: null,
        },
        {
          text: "ورود",
          id: true,
        },
        {
          text: "خروج",
          id: false,
        },
      ],
      menu1: false,
      menu2: false,
      attendance: {},
      totalAttendances: 0,
      pages: 0,
      deg: 0,
      name: null,
      address: null,
      attendanceList: [],
      loading: true,
      options: {
        isEnter: null,
        userFullName: null,
        code: null,
        to: null,
        from: null,
        mustSort: true,
        sortDesc: [false],
      },

      headers: [
        { text: "نام و نام خانوادگی", value: "userFullName", sortable: false },
        { text: "ساعت ", value: "userTime", sortable: false },
        { text: "کد ", value: "code", sortable: false },
        { text: "نوع", value: "entered", sortable: false },
        { text: "تصویر", value: "enterImage", sortable: false },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getAttendances();
      },
      deep: true,
    },
  },
  computed: {
    formattedDate: function () {
      // !! format the date based on this.currentLocale !!
      let formattedDate = "";
      let options = {
        weekday: "short",
        year: "numeric",
        month: "2-digit",
        day: "numeric",
      };
      if (this.options.from) {
        formattedDate = new Date(this.options.from).toLocaleDateString(
          "fa",
          options
        );
      }
      return formattedDate;
    },
    formattedDate1: function () {
      // !! format the date based on this.currentLocale !!
      let formattedDate = "";
      let options = {
        weekday: "short",
        year: "numeric",
        month: "2-digit",
        day: "numeric",
      };
      if (this.options.to) {
        formattedDate = new Date(this.options.to).toLocaleDateString(
          "fa",
          options
        );
      }
      return formattedDate;
    },
  },
  mounted() {
    this.getAttendances();
  },

  methods: {
    getModal(img) {
      this.$refs.modal.openModel(img);
    },
    next(page) {
      this.options.page = page;
      this.getAttendances();
    },
    handler(event) {
      this.options = event;
    },
    clearFrom() {
      this.options.from = null;
    },
    clearTo() {
      this.options.to = null;
    },
    async getAttendances() {
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
        .get("/attendance/attendances?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.attendanceList = data.result;
          this.totalAttendances = data.totalItems;
          this.pages = data.pageCount;
        })
        .catch((error) => {
          alert(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },

    submit() {
      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");
      this.loading = true;
      request
        .get(`/attendance/attendence-excel?${filterQuery}`, {
          responseType: "blob",
        })
        .then((response) => {
          if (response.data.size <= 0) {
            alert("دیتایی برای دانلود یافت نشد");
            return;
          }
          var fileURL = window.URL.createObjectURL(new Blob([response.data]));
          var fileLink = document.createElement("a");

          fileLink.href = fileURL;
          fileLink.setAttribute("download", "file.xlsx");
          document.body.appendChild(fileLink);
          fileLink.click();
        })
        .finally(() => {
          this.loading = false;
        });
    },
  },
};
</script>
<style scoped>
#myModal {
  display: none; /* Hidden by default */
  width: 60%;
  height: 600px;
  position: absolute;
  right: 18%;
  bottom: 14%;
}
/* Modal Content (image) */
</style>
