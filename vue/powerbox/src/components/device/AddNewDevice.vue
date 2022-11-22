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
      <template  v-slot:activator="{ on, attrs }">
        <v-btn color="primary" dark v-bind="attrs" v-on="on"
          > افزودن دستگاه</v-btn
        >
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">دستگاه جدید</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    outlined
                    clearable
                    v-model="device.name"
                    label="نام  *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="nameRules"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    outlined
                    clearable
                    v-model="device.hardwareCode"
                    label="نام سخت افزاری *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="hardwareCodeRules"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    outlined
                    clearable
                    v-model="device.advertisingCode"
                    label="نام دستگاه تبلیغات *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="advertisingCodeRules"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>

              <v-row>
                <v-col cols="12" sm="12" md="3">
                  <v-text-field
                    outlined
                    type="number"
                    clearable
                    v-model="device.lockerCount"
                    label="تعداد لاکرها *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="lockerCountRules"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="12" sm="12" md="3">
                  <v-text-field
                    :disabled="device.freeSms"
                    outlined
                    type="number"
                    clearable
                    v-model="device.smsCredit"
                    label="اعتبار پیامک"
                    placeholder=" "
                    autocomplete="false"
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="12" sm="12" md="3">
                  <v-switch
                    v-model="device.sendSmsStatus"
                    :label="`ارسال پیامک: ${
                      device.sendSmsStatus ? 'فعال' : 'غیر فعال'
                    }`"
                  ></v-switch>
                </v-col>
                <v-col cols="12" sm="12" md="3">
                  <v-switch
                    v-model="device.freeSms"
                    :label="`پیامک رایگان: ${
                      device.freeSms ? 'فعال' : 'غیر فعال'
                    }`"
                  ></v-switch>
                </v-col>
                <v-col cols="12" sm="12" md="3">
                  <v-switch
                    v-model="device.signShow"
                    :label="`نمایش امضا: ${
                      device.signShow ? 'فعال' : 'غیر فعال'
                    }`"
                  ></v-switch>
                </v-col>
              </v-row>

              <v-row>
                <v-col cols="12" sm="12" md="3">
                  <v-switch
                    v-model="device.powerBoxSign"
                    :label="`امضای پاورباکس: ${
                      device.powerBoxSign ? 'فعال' : 'غیر فعال'
                    }`"
                  ></v-switch>
                </v-col>
                <v-col cols="12" sm="12" md="3">
                  <v-switch
                    v-model="device.isFullTime"
                    :label="`ساعت کاری : ${
                      device.isFullTime ? '24 ساعته' : ' محدود'
                    }`"
                  ></v-switch>
                </v-col>
              </v-row>

              <div v-if="!device.isFullTime">
                <h2>ساعات کار روزانه :</h2>
                <br />

                <v-row>
                  <v-col cols="12" sm="12" md="4">
                    <h3>شروع :</h3>
                    <v-time-picker
                      v-model="device.startTime"
                      elevation="15"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>

                  <v-col cols="12" sm="12" md="4">
                    <h3>پایان :</h3>
                    <v-time-picker
                      label="sdf"
                      elevation="15"
                      v-model="device.endTime"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>
                </v-row>
              </div>
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
  name: "AddNewDevices",
  components: {},
  data: () => ({
    dialog: false,
    valid: false,
    deviceId: "",
    loading: false,
    device: {
      complexId: "",
      name: "",  
      hardwareCode: "",
      advertisingCode: "",
      startTime: "",
      endTime: "",
      lockerCount: 0,
      sendSmsStatus: false,
      freeSms: false,
      signShow: false,
      powerBoxSign: false,
      smsCredit: "",
      isFullTime: false,
    },
    nameRules: [(v) => !!v || "لطفا نام را وارد نمایید"],
    lockerCountRules: [(v) => !!v || "لطفا تعداد را وارد نمایید"],
    advertisingCodeRules: [(v) => !!v || "لطفا کد تبلیغات را وارد نمایید"],
    hardwareCodeRules: [(v) => !!v || "لطفا نام سخت افزاری را وارد نمایید"],
  }),
  created() {
    this.device.complexId = this.$route.params.id;
  },
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          (this.deviceId = null),
            (this.device.name = ""),
            (this.device.hardwareCode = ""),
            (this.device.advertisingCode = ""),
            (this.device.startTime = ""),
            (this.device.endTime = ""),
            (this.device.lockerCount = 0),
            (this.device.sendSmsStatus = false),
            (this.device.freeSms = false),
            (this.device.signShow = false),
            (this.device.powerBoxSign = false),
            (this.device.smsCredit = 0),
            (this.device.isFullTime = false);
        }
        if (this.deviceId) this.getDevice(this.deviceId);
      },
      deep: true,
    },
  },
  methods: {
    async getDevice(id) {
      await request.get(`/device/${id}`).then((response) => {
        var data = response.data.result;
        (this.device.name = data.name),
          (this.device.hardwareCode = data.hardwareCode),
          (this.device.advertisingCode = data.advertisingCode),
          (this.device.startTime = data.startTime),
          (this.device.endTime = data.endTime),
          (this.device.lockerCount = data.lockerCount),
          (this.device.sendSmsStatus = data.sendSmsStatus),
          (this.device.freeSms = data.freeSms),
          (this.device.signShow = data.signShow),
          (this.device.powerBoxSign = data.powerBoxSign),
          (this.device.smsCredit = data.smsCredit),
          (this.device.isFullTime = data.isFullTime);
      });
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      if (this.deviceId) {
        request
          .put(`/device/${this.deviceId}`, this.device)
          .then(() => {
            this.dialog = false;
            this.$emit("reloadDevices");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/device", this.device)
          .then(() => {
            this.dialog = false;
            this.$emit("reloadDevices");
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
