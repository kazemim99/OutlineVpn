

<template>
  <v-container class="pa-4">
    <!-- Protocol Selection and User Data Section -->
    <v-row>
      <v-col cols="12" md="8">
        <v-select
          v-model="userData.selectedProtocol"
          :items="protocols"
          item-text="text"
          item-value="id"
          label="پروتکل ها"
          outlined
        ></v-select>
        <v-btn color="primary" class="ml-3" @click="changeProtocol"
          >تغییر کانفیگ</v-btn
        >
      </v-col>
      <v-col cols="12" md="4">
        <v-card class="pa-3">
          <div>نام کاربری: {{ userData.username }}</div>
          <div>رمز عبور: {{ userData.password }}</div>
          <div>سرور : {{ userData.server }}</div>
          <div>تاریخ انقضا: {{ userData.expireDate }}</div>
          <div>ترافیک مصرف شده : {{ userData.trafficUsage }}</div>
          <div>ترافیک کل : {{ userData.totalTraffic }}</div>
        </v-card>
      </v-col>
    </v-row>

    <!-- QR Code and Config String Section -->
    <v-row class="mt-4">
      <v-col cols="12" md="6">
        <div class="d-flex justify-center align-center">
          <!-- QR Code component, dynamically generated based on configString -->
          <qrcode-vue :value="userData.configString" :size="250"></qrcode-vue>
        </div>
      </v-col>

      <v-col cols="12" md="6">
        <v-text-field
          v-model="userData.configString"
          label="لینک کانفیگ"
          outlined
          readonly
          append-icon="mdi-content-copy"
          @click:append="copyConfigString"
        ></v-text-field>
      </v-col>
    </v-row>

    <v-row class="mt-2">
      <v-col cols="12">
        <div class="d-flex align-center justify-center">
          <span class="download-title">دانلود اپلیکیشن</span>
        </div>
        <v-divider></v-divider>
      </v-col>

      <v-col cols="12" sm="4">
        <v-btn color="primary" @click="downloadApp('ios')" outlined block>
          <v-icon left>mdi-apple</v-icon> iOS App
        </v-btn>
      </v-col>
      <v-col cols="12" sm="4">
        <v-btn color="green" @click="downloadApp('android')" outlined block>
          <v-icon left>mdi-android</v-icon> Android App
        </v-btn>
      </v-col>
      <v-col cols="12" sm="4">
        <v-btn color="blue" @click="downloadApp('windows')" outlined block>
          <v-icon left>mdi-microsoft-windows</v-icon> Windows App
        </v-btn>
      </v-col>
    </v-row>

    <v-row class="mt-5">
      <v-col cols="12">
        <div class="d-flex align-center justify-center">
          <span class="download-title"> ویدئو آموزشی</span>
        </div>
        <v-divider></v-divider>
      </v-col>

      <v-col cols="12" sm="4">
        <v-btn color="primary" @click="downloadApp('ios')" outlined block>
          <v-icon left>mdi-play</v-icon> ویدئو آموزش آیفون
        </v-btn>
      </v-col>
      <v-col cols="12" sm="4">
        <v-btn color="green" @click="downloadApp('android')" outlined block>
          <v-icon left>mdi-play</v-icon> ویدئو آموزش اندروید
        </v-btn>
      </v-col>
      <v-col cols="12" sm="4">
        <v-btn color="blue" @click="downloadApp('windows')" outlined block>
          <v-icon left>mdi-play</v-icon>ویدئو آموزش ویندوز
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import request from "@/utils/request";
import QrcodeVue from "qrcode-vue"; // Import QR code component

export default {
  components: {
    QrcodeVue,
  },
  data() {
    return {
      protocols: [], // Protocol options
      configString: "", // Config string received from server
      guid: null,
      userData: {
        username: "xxxx",
        expireDate: "asdfd",
        trafficUsage: "23 gig",
        configString: "",
        totalTraffic: "",
        selectedProtocol: null,
      },
      videoUrl: "", // URL for the video player
    };
  },

  created() {
    this.fetchProtocols();
  },

  methods: {
    async getInfo() {
      this.guid = this.$route.query.guid;
      await request.get(`PublicData/profile/${this.guid}`).then((response) => {
        this.userData = response.data.result;
      });
    },

    async fetchProtocols() {
      await request.get(`PublicData/get-accounType`).then((response) => {
        this.protocols = response.data.result;
        this.getInfo();
      });
    },
    async changeProtocol() {
      // Call server to get config string based on selected protocol
      if (this.userData.selectedProtocol) {
        await request
          .get(
            `PublicData/ChangeConfig/${this.guid}/${this.userData.selectedProtocol}`
          )
          .then(() => {
            this.getInfo();
          });
      }
    },
    copyConfigString() {
      // Copy configString to clipboard
      navigator.clipboard.writeText(this.userData.configString);
      // this.$toast.success("Config string copied to clipboard!");
    },
    downloadApp(platform) {
      // Redirect to download link based on platform
      let url = "";
      switch (platform) {
        case "ios":
          url = "/download/ios";
          break;
        case "android":
          url = "/download/android";
          break;
        case "windows":
          url = "https://sourceforge.net/projects/netmodhttp/";
          break;
      }
      window.open(url, "_blank");
    },
    async fetchVideoUrl() {
      // Fetch video URL from server
      const response = await fetch("/api/videoUrl");
      this.videoUrl = await response.text();
      this.videoUrl =
        "https://s31.uupload.ir/files/toturial/Recorder_27102024_214848.mp4";
    },
  },
  mounted() {
    this.fetchProtocols(); // Fetch protocols on mount
    this.fetchVideoUrl(); // Fetch video URL on mount
  },
};
</script>


<style scoped>
.pa-4 {
  padding: 16px;
}

.mt-4 {
  margin-top: 16px;
}

.mt-2 {
  margin-top: 8px;
}
</style>

