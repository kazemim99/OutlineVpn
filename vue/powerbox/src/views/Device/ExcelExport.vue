<template>
  <v-form ref="form" v-model="valid" lazy-validation>
    <v-container>
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
            @change="getDevices(selectedSubComplexId)"
            solo
          ></v-select>
        </v-col>
        <v-col cols="4">
          <v-select
            v-model="support.deviceId"
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
              v-model="support.from"
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
              v-model="support.to"
              @input="menu2 = false"
            ></v-date-picker>
          </v-menu>
        </v-col>
      </v-row>

      <v-spacer></v-spacer>
      <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
        >دریافت</v-btn
      >
    </v-container>
  </v-form>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";

export default Vue.extend({
  name: "ExcelExport",
  components: {},
  data: () => ({
    isUserEdit: false,
    isComplexEdit: false,
    complexNameInput: "",
    userNameInput: "",
    supportId: null,
    selectedComplex: {},
    selectedUser: {},
    complexes: [],
    subComplexes: [],
    selectedComplexId: null,
    selectedSubComplexId: null,
    users: [],
    dialog: false,
    valid: true,
    loading: false,
    imageUrl: "",
    isUnit: false,
    support: {
      from: "",
      to: "",
      deviceId: "",
      complexId: "",
    },
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          this.support.startTime = null;
          this.support.endTime = null;
          this.support.deviceId = null;
          this.support.complexId = null;
          this.selectedSubComplexId = null;
          this.selectedComplexId = null;

          this.supportId = null;
        }
        if (this.supportId) {
          this.getSupport(this.supportId);
        }
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
      if (this.support.from) {
        formattedDate = new Date(this.support.from).toLocaleDateString(
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
      if (this.support.to) {
        formattedDate = new Date(this.support.to).toLocaleDateString(
          "fa",
          options
        );
      }
      return formattedDate;
    },
  },
  created() {
    this.getComplexes();
  },

  methods: {
    clearFrom() {
      this.support.from = null;
    },
    clearTo() {
      this.support.to = null;
    },
    complexFilter(item, queryText, itemText) {
      const searchText = queryText.toLowerCase();
    },
    userFilter(item, queryText, itemText) {
      const searchText = queryText.toLowerCase();
    },
    async getComplexes() {
      await request.get(`/publicData/main-complexes`).then((response) => {
        var data = response.data.result;
        this.complexes = data;
        this.complexes.unshift({ id: null, text: "انتخاب..." });
      });
    },
    async getSubComplexes() {
      if (this.selectedComplexId) {
        await request
          .get(`/publicData/sub-complexes/${this.selectedComplexId}`)
          .then((response) => {
            var data = response.data.result;
            this.subComplexes = data;
            this.subComplexes.unshift({ id: null, text: "انتخاب..." });
            this.getDevices(this.selectedComplexId);
          });
      }
    },
    async getDevices(complexId) {
      if (complexId) {
        await request
          .get(
            `/publicData/main-complexes-devices/${complexId}`
          )
          .then((response) => {
            var data = response.data.result;
            this.users = data;
            this.users.unshift({ id: null, text: "انتخاب..." });
          });
      }
    },

    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      if (!this.support.from) {
        alert("لطفا تاریخ شروع را وارد نمایید");
        return;
      }
      if (!this.support.to) {
        alert("لطفا تاریخ پایان را وارد نمایید");
        return;
      }
      this.support.selectedComplexId = this.selectedComplexId;
      this.support.selectedSubComplexId = this.selectedSubComplexId;

      const filterQuery = Object.keys(this.support)
        .filter(
          (x) => this.support[x] !== null && this.support[x] !== undefined
        )
        .map((key) => `${key}=${this.support[key]}`)
        .join("&");
      this.loading = true;
      request
        .get(`/deviceManagement/excel-log?${filterQuery}`, {
          responseType: "blob",
        })
        .then((response) => {
          if (response.data.size <= 0) {
            alert("دیتایی برای دانلود یافت نشد");
            return;
          }
          this.dialog = false;
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
});
</script>

<style scoped>
.v-card--reveal {
  align-items: center;
  bottom: 0;
  justify-content: center;
  opacity: 0.5;
  position: absolute;
  width: 100%;
}

.card-form-img {
  padding: 0px !important;
}

.icon-btn-modal {
  position: absolute;
  font-size: 18px !important;
  color: #fff !important;
  padding: 8px;
  border-radius: 50%;
}

.icon-btn-modal:hover {
  cursor: pointer;
}

.icon-btn-upload {
  position: absolute;
  left: 60%;
  bottom: 33%;
  color: #fff !important;
  /*padding: 8px;*/
  border-radius: 50%;
  /*background: #35495E !important;*/
  text-align: center;
  display: flex;
  margin: auto;
  justify-content: center;
  align-items: center;
  /*height: 20px !important;*/
  /*width: 20px !important;*/
}

.v-icon {
  color: #fff !important;
  font-size: 18px !important;
  text-align: center;
  background: #35495e !important;
}

.logo-title {
  text-align: center;
  display: flex;
  justify-content: center;
  margin-bottom: 15px;
}
</style>
