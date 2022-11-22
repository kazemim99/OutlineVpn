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
    <v-row>
      <v-col sm="12" md="2">
        <v-text-field
          v-model="filter.lockerNo"
          class="pa-4"
          type="text"
          label="شماره لاکر"
        ></v-text-field>
      </v-col>

      <v-col sm="12" md="2">
        <v-select
          v-model="filter.cableType"
          :items="cableTypes"
          multiple
          item-value="id"
          item-text="text"
          label="نوع کابل"
        ></v-select>
      </v-col>
      <v-col sm="12" md="2">
        <v-select
          v-model="filter.lockerType"
          :items="lockTypes"
          multiple
          item-value="id"
          item-text="text"
          label="نوع قفل"
        ></v-select>
      </v-col>
      <v-col sm="12" md="2">
        <v-select
          v-model="filter.rate"
          :items="getRates()"
          multiple
          item-value="id"
          item-text="text"
          label="شماره امتیاز"
        ></v-select>
      </v-col>
      <v-col sm="12" md="2">
        <v-select
          v-model="filter.receiveType"
          :items="receiveTypesModel"
          multiple
          item-value="id"
          item-text="text"
          label="نوع دریافت"
        ></v-select>
      </v-col>
    </v-row>
    <v-row>
      <v-col sm="12" md="3">
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
              @click:clear="clearStartDate()"
              v-model="formattedDate"
              label="تاریخ اغاز"
              prepend-icon="mdi-calendar"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            :first-day-of-week="0"
            locale="fa-ir"
            v-model="filter.startPicker"
            @input="menu1 = false"
          ></v-date-picker>
        </v-menu>
      </v-col>
      <v-col sm="12" md="3">
        <v-menu
          ref="menu2"
          @click:clear="clearStartTime()"
          v-model="menu2"
          :close-on-content-click="false"
          :nudge-right="40"
          :return-value.sync="filter.startTime"
          transition="scale-transition"
          offset-y
          max-width="290px"
          min-width="290px"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              clearable
              v-model="filter.startTime"
              label="ساعت اغاز"
              prepend-icon="mdi-clock-time-four-outline"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-time-picker
            v-if="menu2"
            v-model="filter.startTime"
            @click:minute="$refs.menu2.save(filter.startTime)"
          ></v-time-picker>
        </v-menu>
      </v-col>

      <v-col sm="12" md="3">
        <v-menu
          v-model="menu3"
          :close-on-content-click="false"
          :nudge-right="40"
          transition="scale-transition"
          offset-y
          min-width="auto"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              @click:clear="clearEndDate()"
              clearable
              v-model="formattedDate1"
              label="تاریخ دریافت"
              prepend-icon="mdi-calendar"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            :first-day-of-week="0"
            locale="fa-ir"
            v-model="filter.endPicker"
            @input="menu3 = false"
          ></v-date-picker>
        </v-menu>
      </v-col>
      <v-col sm="12" md="3">
        <v-menu
          ref="menu4"
          v-model="menu4"
          :close-on-content-click="false"
          :nudge-right="40"
          :return-value.sync="filter.endTime"
          transition="scale-transition"
          offset-y
          max-width="290px"
          min-width="290px"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              @click:clear="clearEndTime()"
              clearable
              v-model="filter.endTime"
              label="ساعت پایان"
              prepend-icon="mdi-clock-time-four-outline"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-time-picker
            v-if="menu4"
            v-model="filter.endTime"
            @click:minute="$refs.menu4.save(filter.endTime)"
          ></v-time-picker>
        </v-menu>
      </v-col>
    </v-row>
    <v-row>
      <v-col sm="12" md="2">
        <v-text-field
          v-model="filter.phoneNumber"
          label="شماره موبایل"
        ></v-text-field>
      </v-col>
    </v-row>
    <v-btn @click="submit()" class="mx-10 mt-5 mb-10" small dark color="indigo">
      اعمال فیلتر
    </v-btn>
    <v-spacer></v-spacer>
    <v-data-table
      @click:row="getLocerLogs"
      :headers="_headers"
      :items="lockers"
      :loading="loading"
      :server-items-length="totalLockers"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.chargeImageData="{ item }">
        <img
          @click="getModal(item.chargeImageData)"
          :src="item.chargeImageData"
          width="50"
          height="50"
        />
      </template>

      <template v-slot:item.permanentImageData="{ item }">
        <img
          @click="getModal(item.permanentImageData)"
          :src="item.permanentImageData"
          width="50"
          height="50"
        />
      </template>

      <template v-slot:item.receiveImageData="{ item }">
        <img
          @click="getModal(item.receiveImageData)"
          :src="item.receiveImageData"
          width="50"
          height="50"
        />
      </template>

      <template v-slot:item.lockImageData="{ item }">
        <img
          @click="getModal(item.lockImageData)"
          :src="item.lockImageData"
          width="50"
          height="50"
        />
      </template>
    </v-data-table>

    <v-pagination
      v-model="options.page"
      @input="next"
      :length="this.pages"
      :total-visible="7"
    ></v-pagination>
    <rotate ref="modal"></rotate>

    <v-dialog
      v-model="dialog"
      transition="dialog-top-transition"
      max-width="600"
    >
      <v-card>
        <v-card-title class="text-h5"> جزییات </v-card-title>

        <v-card-text>
          <p>شماره لاکر : {{ details.lockerNo }}</p>
          <br />
          <p>وضعیت : {{ details.state }}</p>
          <br />
          <p>شماره تلفن : {{ details.phoneNumber }}</p>
          <br />
          <p>عکس امنیتی : {{ details.securityImage }}</p>
          <br />
          <p>زمان آغاز : {{ details.startDateTime }}</p>
          <br />
          <p>زمان پایان : {{ details.endDateTime }}</p>
          <br />
          <p>زمان قفل موقت : {{ details.temporaryLockTime }}</p>
          <br />
          <p>زمان قفل دائم : {{ details.alwaysLockTime }}</p>
          <br />
          <p>نوع دریافت : {{ details.receiveType }}</p>
          <br />
          <p>امتیاز : {{ details.ratedStars }}</p>
          <v-row>
            <div>
              <span>عکس کاربر دریافت : </span>

              <img
                @click="getModal(details.lockImageData)"
                :src="details.lockImageData"
                width="20"
                height="20"
              />
            </div>

            <div>
              <span>عکس کاربر قفل موقت : </span>
              <img
                @click="getModal(details.receiveImageData)"
                :src="details.receiveImageData"
                width="20"
                height="20"
              />
            </div>
          </v-row>
          <v-row>
            <div>
              <span>عکس کاربر قفل دائم : </span>
              <img
                @click="getModal(details.permanentImageData)"
                :src="details.permanentImageData"
                width="20"
                height="20"
              />
            </div>

            <div>
              <span> عکس کاربر شارژ : </span>

              <img
                  @click="getModal(details.chargeImageData)"
                :src="details.chargeImageData"
                width="20"
                height="20"
              />
            </div>
          </v-row>
        </v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" text @click="dialog = false">
            بستن
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>
<script>
import request from "@/utils/request";
import Breadcrump from "@/components/common/Breadcrump.vue";
import rotate from "@/components/common/Rotate.vue";

import qs from "qs";

export default {
  name: "DeviceReport",
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
          text: "مجموعه ها",
          disabled: false,
          href: "/complexes",
        },
        {
          text: `مجموعه ${this.$store.state.selectedComplexName}`,
          disabled: false,
          href: `/complex-units/${this.$store.state.selectedComplexId}`,
        },
        {
          text: `${this.$store.state.selectedDeviceName}`,
          href: `/complex-devices/${this.$route.params.id}`,
          disabled: false,
        },
        {
          text: "'گزارش استفاده'",
          disabled: true,
        },
      ],
      details: {},
      name: null,
      dialog: false,
      lockers: [],
      menu1: false,
      menu2: false,
      menu3: false,
      menu4: false,

      cableTypes: [
        {
          text: "Apple",
          id: "Apple",
        },
        {
          text: "Android",
          id: "Android",
        },
      ],
      lockTypes: [
        {
          text: "قفل دائم",
          id: 1,
        },
        {
          text: "قفل موقت",
          id: 0,
        },
      ],
      receiveTypesModel: [],
      totalLockers: 0,
      pages: 0,
      options: {
        mustSort: true,
        sortDesc: [false],
      },
      filter: {
        startPicker: null,
        startTime: null,
        endPicker: null,
        endTime: null,
        lockerNo: null,
        lockerType: [],
        rate: [],
        cableType: [],
        phoneNumber: null,
        receiveType: [],
      },
      loading: false,
      headersMobile: [
        {
          text: "شماره لاکر",
          show: this.getIdMobile,
          value: "lockerNo",
          sortable: false,
        },
        { text: "وضعیت", value: "state", sortable: false },
        { text: " شماره تلفن", value: "phoneNumber", sortable: false },
      ],

      headers: [
        {
          text: "شماره لاکر",
          show: this.getIdMobile,
          value: "lockerNo",
          sortable: false,
        },
        { text: "وضعیت", value: "state", sortable: false },
        { text: "شناسه", align: " d-none", value: "id", sortable: false },
        { text: " شماره تلفن", value: "phoneNumber", sortable: false },
        { text: "عکس امنیتی", value: "securityImage", sortable: false },
        { text: "زمان آغاز", value: "startDateTime", sortable: false },
        { text: "زمان پایان", value: "endDateTime", sortable: false },
        { text: "نوع کابل ", value: "cableType", sortable: false },
        { text: "زمان قفل موقت", value: "temporaryLockTime", sortable: false },
        { text: "زمان قفل دائم", value: "alwaysLockTime", sortable: false },
        { text: "نوع دریافت", value: "receiveType", sortable: false },
        { text: "امتیاز", value: "ratedStars", sortable: false },

        {
          text: "عکس کاربر دریافت",
          value: "receiveImageData",
          sortable: false,
        },
        {
          text: "عکس کاربر قفل موقت",
          value: "lockImageData",
          sortable: false,
        },
        {
          text: "عکس کاربر قفل دائم",
          value: "permanentImageData",
          sortable: false,
        },
        {
          text: "عکس کاربر شارژ",
          value: "chargeImageData",
          sortable: false,
        },
        { text: "", value: "openLocker", sortable: false, width: "1%" },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getLockers();
      },
      deep: true,
    },
  },
  computed: {
    _headers() {
      if (this.IsMobile()) {
        return this.headersMobile;
      }
      return this.headers;
    },

    formattedDate: function () {
      // !! format the date based on this.currentLocale !!
      let formattedDate = "";
      let options = {
        weekday: "short",
        year: "numeric",
        month: "2-digit",
        day: "numeric",
      };
      if (this.filter.startPicker) {
        formattedDate = new Date(this.filter.startPicker).toLocaleDateString(
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
      if (this.filter.endPicker) {
        formattedDate = new Date(this.filter.endPicker).toLocaleDateString(
          "fa",
          options
        );
      }
      return formattedDate;
    },
  },
  created() {
    this.deviceId = this.$route.params.id;
    this.getLockers();
    this.getReceiveTypes();
  },
  methods: {
    IsMobile() {
      if (screen.width <= 760) {
        return true;
      } else {
        return false;
      }
    },
    getModal(img) {
      this.$refs.modal.openModel(img);
    },
    clearStartTime() {
      this.filter.startTime = null;
    },
    clearEndTime() {
      this.filter.endTime = null;
    },
    clearStartDate() {
      this.filter.startPicker = null;
    },
    clearEndDate() {
      this.filter.endPicker = null;
    },
    submit() {
      this.getLockers();
    },

    getLocerLogs(object) {
      request
        .get(`/deviceManagement/log-by-id/${object.id}`)
        .then((response) => {
          if (!this.IsMobile()) return;
          var resp = response.data.result;
          this.details.lockerNo = resp.lockerNo;
          this.details.state =
            resp.state ;
            this.details.phoneNumber =  resp.phoneNumber;
          this.details.securityImage = resp.securityImage;
          this.details.startDateTime = resp.startDateTime;
          this.details.endDateTime = resp.endDateTime;
          this.details.cableType = resp.cableType;
          this.details.temporaryLockTime = resp.temporaryLockTime;
          this.details.alwaysLockTime = resp.alwaysLockTime;
          this.details.receiveType = resp.receiveType;
          this.details.ratedStars = resp.ratedStars;
          this.details.chargeImageData = resp.chargeImageData;
          this.details.permanentImageData = resp.permanentImageData;
          this.details.receiveImageData = resp.receiveImageData;
          this.details.lockImageData = resp.lockImageData;
          this.dialog = true;
        });
    },
    async getReceiveTypes() {
      await request.get(`/publicData/receive-types`).then((response) => {
        this.receiveTypesModel = response.data.result;
      });
    },
    getRates() {
      let rates = [];
      for (let i = 1; i <= 5; i++) {
        rates.push({
          id: parseInt(i),
          text: `${i}`,
        });
      }
      return rates;
    },
    next(page) {
      this.options.page = page;
      this.getLockers();
    },
    async getLockers() {
      this.loading = true;
      const { sortDesc, sortBy, page, itemsPerPage } = this.options;
      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");
      this.loading = true;
      await request
        .get(`/deviceManagement/device-log/${this.deviceId}?` + filterQuery, {
          params: {
            lockerType: this.filter.lockerType,
            cableType: this.filter.cableType,
            rate: this.filter.rate,
            receiveType: this.filter.receiveType,
            startPicker: this.filter.startPicker,
            startTime: this.filter.startTime,
            endPicker: this.filter.endPicker,
            endTime: this.filter.endTime,
            lockerNo: this.filter.lockerNo,
            phoneNumber: this.filter.phoneNumber,
          },
          paramsSerializer: (params) => {
            return qs.stringify(params, { arrayFormat: "repeat" });
          },
        })
        .then((response) => {
          this.loading = false;

          var data = response.data.result;
          this.lockers = data.result;
          console.log(data.result);
          this.totalLockers = data.totalItems;
          this.pages = data.pageCount;
        })
        .catch((error) => {
          alert(error);
        });
    },
  },
};
</script>
