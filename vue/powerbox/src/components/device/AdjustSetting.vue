<template>
  <v-row justify="center">
    <v-dialog
      v-model="dialog"
      hide-overlay
      transition="dialog-bottom-transition"
      persistent
      max-width="600px"
    >
      <template v-slot:activator="{ on, attrs }">
        <v-btn
        v-can="'Devices_Adjust'"
          v-bind="attrs"
          v-on="on"
          small
          color="primary"
          class="white--text"
        >
          یکسان سازی
          <v-icon right dark> mdi-adjust </v-icon>
        </v-btn>
      </template>

      <v-card>
        <v-card-title>
          <span class="text-h5">یکسان سازی</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-col cols="12" sm="6">
                <v-select
                  v-model="model.deviceIds"
                  :items="devices"
                  item-value="id"
                  item-text="text"
                  attach
                  chips
                  label="دستگاه ها"
                  multiple
                ></v-select>
              </v-col>
              <v-row>
                <v-col cols="4">
                  <v-checkbox
                    v-model="model.content"
                    label="محتوا"
                    color="red"
                    hide-details
                  ></v-checkbox>
                </v-col>
                <v-col cols="4">
                  <v-checkbox
                    v-model="model.sms"
                    label="پیامک"
                    color="red"
                    hide-details
                  ></v-checkbox>
                </v-col>
                <v-col cols="4">
                  <v-checkbox
                    v-model="model.activityCode"
                    label="کدهای فعالیت"
                    color="red"
                    hide-details
                  ></v-checkbox>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false"
            >انصراف</v-btn
          >
          <v-btn
            :loading="loading"
            color="blue darken-1"
            text
            @click="adjustDevice()"
            >اعمال</v-btn
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
  name: "AdjustSetting",
  props: ["selectedComplexId", "currentDevice"],

  components: {},
  data: () => ({
    dialog: false,
    complexId: null,
    activityCode: false,
    devices: [],

    model: {
      sms: false,
      content: false,
      deviceIds: [],
    },
  }),
  created() {
    this.getDevices();
  },

  methods: {
    async getDevices() {
      await request
        .get(`/device/complex-devices/${this.selectedComplexId}`)
        .then((response) => {
          var data = response.data.result;
          this.devices = data;
          this.devices.push({
            id: -1,
            text: "همه",
          });
        });
    },

    async adjustDevice() {
      await request
        .put(
          `/device/adjust-setting/${this.currentDevice}/${this.selectedComplexId}`,
          this.model
        )
        .then((response) => {
          this.dialog = false;
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
