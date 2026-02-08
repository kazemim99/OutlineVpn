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
    <!-- <v-container>
      <v-row>
        <v-col cols="4">
          <v-select
            v-model="selectedComplexId"
            :items="complexes"
            item-value="id"
            item-text="text"
            label="مجموعه"
            @change="getSubComplexes"
            solo
          ></v-select>
        </v-col>

        <v-col cols="4">
          <v-select
            v-model="selectedSubComplexId"
            :items="subComplexes"
            item-value="id"
            item-text="text"
            label="زیر مجموعه"
            @change="getDevices"
            solo
          ></v-select>
        </v-col>
        <v-col cols="4">
          <v-select
            v-model="options.deviceId"
            :items="users"
            item-value="id"
            item-text="text"
            label="دستگاه"
            solo
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
    </v-container> -->
    <v-data-table
      :headers="headers"
      :items="logList"
      :loading="loading"
      :server-items-length="totalDevices"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
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
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";

export default {
  name: "Devices",
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
          text: "لاگ کاربران",
          disabled: false,
          href: "/complexes",
        },
      ],
      selectedComplex: {},
      selectedUser: {},
      complexes: [],
      subComplexes: [],
      selectedComplexId: null,
      selectedSubComplexId: null,
      users: [],
      totalLogs: 0,
      pages: 0,
      name: null,
      logList: [],
      loading: true,

      options: {
        // selectedComplexId: null,
        // selectedSubComplexId: null,
        // from: "",
        // to: "",
        // deviceId: "",
        // complexId: "",
        mustSort: true,
        sortDesc: [false],
      },

      headers: [
        { text: "شناسه", value: "id", sortable: false },
        { text: "تاریخ", value: "date", sortable: false },
        { text: "متن", value: "content", sortable: false },
      ],
    };
  },

  watch: {
    options: {
      handler() {
        this.getLogs();
      },
      deep: true,
    },
    // name: function () {
    //   if (this.name.length > 2 || this.name.length === 0) {
    //     this.options.page = 1;
    //     this.options.name = this.name;
    //     this.getDevices();
    //   }
    // },
  },
  computed: {
    formattedDate: function () {
      // !! format the date based on this.currentLocale !!
      let formattedDate = "";
      const options = {
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
      const options = {
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
    this.getLogs();
  },

  methods: {
    clearFrom() {
      this.support.from = null;
    },
    clearTo() {
      this.support.to = null;
    },
    next(page) {
      this.options.page = page;
      this.getLogs();
    },
    handler(event) {
      this.options = event;
    },
    async getComplexes() {
      await request.get(`/publicData/main-complexes`).then((response) => {
        const data = response.data.result;
        this.complexes = data;
      });
    },
    async getSubComplexes() {
      await request
        .get(`/publicData/sub-complexes/${this.selectedComplexId}`)
        .then((response) => {
          const data = response.data.result;
          this.subComplexes = data;
          this.selectedSubComplexId = this.selectedComplexId;
          this.getDevices();
        });
    },
    async getDevices(name) {
      await request
        .get(`/publicData/main-complexes-devices/${this.selectedSubComplexId}`)
        .then((response) => {
          const data = response.data.result;
          this.users = data;
        });
    },
    async getLogs() {
      const { sortDesc, sortBy, page, itemsPerPage } = this.options;
      this.loading = true;

      this.options.selectedComplexId = this.selectedComplexId;
      this.options.selectedSubComplexId = this.selectedSubComplexId;

      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");

      this.loading = true;
      await request
        .get(`/user/member-log?${filterQuery}`)
        .then((response) => {
          const data = response.data.result;
          this.logList = data.result;
          this.totalLogs = data.totalItems;
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
