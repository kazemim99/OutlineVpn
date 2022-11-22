<template>
  <v-row justify="center">
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
      persistent
      max-width="600px"
    >
      <template v-can="'Report_Create'" v-slot:activator="{ on, attrs }">
        <v-btn color="primary" dark v-bind="attrs" v-on="on"> افزودن </v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">{{ !reportId ? "افزودن" : "ویرایش " }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-col class="d-flex" cols="12" sm="6">
                <v-select
                  v-model="selectedComplexId"
                  :items="complexes"
                  item-value="id"
                  item-text="text"
                  label="مجموعه"
                  @change="getDevices"
                  solo
                ></v-select>
              </v-col>

              <v-col class="d-flex" cols="12" sm="6">
                <v-select
                  v-model="report.deviceId"
                  :items="devices"
                  item-value="id"
                  item-text="text"
                  label="دستگاه"
                  solo
                ></v-select>
              </v-col>

              <v-col cols="12" sm="12" md="12">
                <v-textarea
                  label="توضیحات "
                  auto-grow
                  v-model="report.content"
                  outlined
                  rows="2"
                  row-height="50"
                  shaped
                ></v-textarea>
              </v-col>
              <v-col cols="6" sm="6">
                <v-file-input
                  v-model="report.file"
                  accept="text/*"
                  label="فایل ضمینه"
                  outlined
                  dense
                ></v-file-input>
              </v-col>
            </v-container>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">بستن</v-btn>
          <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
            >ذخیره</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";

export default Vue.extend({
  name: "AddNewReport",
  components: {},
  data: () => ({
    reportId: null,
    devices: [],
    complexes: [],
    selectedComplex: {},
    selectedDevice: {},
    selectedDeviceId: null,
    selectedComplexId: null,
    isComplexEdit: false,
    isDeviceEdit: false,
    dialog: false,
    valid: true,
    loading: false,
    isUnit: false,
    report: {
      content: "",
      file: "",
      deviceId: "",
    },
  }),
  created() {
    this.getComplexes();
  },

  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          (this.report.content = ""),
            (this.report.file = ""),
            (this.report.deviceId = "");
          this.selectedComplex = {};
          this.selectedDevice = {};
          this.selectedDeviceId = null;
          this.selectedComplexId = null;
          this.reportId = null;
        }
        if (this.reportId) this.getReport(this.reportId);
      },
      deep: true,
    },
  },
  methods: {
    async getComplexes() {
      await request.get(`/publicData/main-complexes`).then((response) => {
        var data = response.data.result;
        this.complexes = data;
      });
    },
    async getDevices() {
      await request
        .get(`/publicData/main-complexes-devices/${this.selectedComplexId}`)
        .then((response) => {
          var data = response.data.result;
          this.devices = data;
        });
    },
    async getReport(id) {
      await request.get(`/report/${id}`).then((response) => {
        var data = response.data.result;
        this.report.content = data.content;
        this.report.deviceId = data.deviceId;
        this.selectedComplexId = data.complexId;
        this.complexes = [{ id: data.complexId, text: data.complexName }];
        this.devices = [{ id: data.deviceId, text: data.deviceName }];
        this.selectedComplex = { id: data.complexId, text: data.complexName };
        this.selectedDevice = { id: data.deviceId, text: data.deviceName };
        this.isComplexEdit = true;
        this.isDeviceEdit = true;
      });
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;
      this.report.userId = this.selectedComplex.id;
      this.report.complexId = this.selectedDevice.id;

      var form_data = new FormData();

      for (var key in this.report) {
        form_data.append(key, this.report[key]);
      }
      request.defaults.headers.common.accept = "multipart/form-data";

      if (this.reportId) {
        request
          .put(`/report/${this.reportId}`, form_data)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadReportes");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/report", form_data)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadReportes");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
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
