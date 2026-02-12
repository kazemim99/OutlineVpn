<template>
  <v-container class="pa-4">
    <!-- Account URL Input Section -->
    <v-row v-if="!accountLoaded">
      <v-col cols="12">
        <v-card class="pa-4">
          <v-card-title>اطلاعات حساب کاربری</v-card-title>
          <v-card-text>
            <v-text-field
              v-model="accountUrl"
              label="لینک اکانت خود را وارد کنید"
              placeholder="vless://..."
              outlined
              hint="مثال: vless://f7bb8195-4291-4f28-b664-7479c007290e@v6.iransshvpn.com:26000?type=ws&path=%2F&host=&security=none#u47652"
              persistent-hint
            ></v-text-field>
            <v-btn
              color="primary"
              class="mt-3"
              @click="loadAccountInfo"
              :loading="loading"
              >بررسی حساب کاربری</v-btn
            >
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Account Information Display -->
    <v-row v-if="accountLoaded">
      <!-- Account Status Card -->
      <v-col cols="12" md="4">
        <v-card class="pa-3">
          <v-card-title class="text-h6">اطلاعات حساب</v-card-title>
          <v-divider class="mb-3"></v-divider>
          <div class="info-row">
            <strong>نام کاربری:</strong> {{ userData.username }}
          </div>
          <div class="info-row">
            <strong>وضعیت:</strong>
            <v-chip
              :color="userData.enable ? 'success' : 'error'"
              small
              class="mr-2"
            >
              {{ userData.enableMessage }}
            </v-chip>
          </div>
        </v-card>
      </v-col>

      <!-- Traffic and Dates Card -->
      <v-col cols="12" md="4">
        <v-card class="pa-3">
          <v-card-title class="text-h6">ترافیک و تاریخ</v-card-title>
          <v-divider class="mb-3"></v-divider>
          <div class="info-row">
            <strong>تاریخ ایجاد:</strong> {{ userData.createDate }}
          </div>
          <div class="info-row">
            <strong>تاریخ انقضا:</strong> {{ userData.expireDate }}
          </div>
          <div class="info-row">
            <v-alert
              :type="userData.isExpired ? 'error' : 'info'"
              dense
              text
              class="mt-2 mb-2"
            >
              {{ userData.expireDateMessage }}
            </v-alert>
          </div>
          <div class="info-row">
            <strong>ترافیک مصرف شده:</strong> {{ userData.usedTraffic }}
          </div>
          <div class="info-row">
            <strong>ترافیک کل:</strong> {{ userData.totalTraffic }}
          </div>
          <div class="info-row">
            <v-alert
              :type="userData.trafficExpired ? 'error' : 'success'"
              dense
              text
              class="mt-2"
            >
              {{ userData.trafficMessage }}
            </v-alert>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <!-- Protocol Selection and User Data Section -->
    <v-row v-if="accountLoaded">
      <v-col cols="12" md="8"></v-col>
    </v-row>

    <!-- QR Code and Config String Section -->
    <v-row class="mt-4">
      <v-col cols="12" md="6">
        <div class="d-flex justify-center align-center">
          <!-- QR Code component, dynamically generated based on configString -->
          <qrcode-vue :value="userData.configString" :size="250"></qrcode-vue>
        </div>
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
      accountUrl: "", // Input for account URL
      accountLoaded: false, // Flag to show account info
      loading: false, // Loading state
      userData: {
        username: "",
        password: "",
        server: "",
        expireDate: "",
        createDate: "",
        usedTraffic: "",
        totalTraffic: "",
        configString: "",
        selectedProtocol: null,
        expireDateMessage: "",
        trafficMessage: "",
        isExpired: false,
        trafficExpired: false,
        enable: false,
        enableMessage: "",
      },
      videoUrl: "", // URL for the video player
    };
  },

  created() {
    this.fetchProtocols();
    // Check if there's a username in the route
    const username = this.$route.query.username;
    if (username) {
      this.loadAccountInfoByUsername(username);
    }
  },

  methods: {
    parseUsername(url) {
      // Parse username from vless URL (after # symbol)
      try {
        const hashIndex = url.indexOf("#");
        if (hashIndex === -1) {
          throw new Error("نام کاربری در لینک یافت نشد");
        }
        const username = url.substring(hashIndex + 1).trim();
        if (!username) {
          throw new Error("نام کاربری خالی است");
        }
        return username;
      } catch (error) {
        this.$toast?.error(error.message || "فرمت لینک اشتباه است");
        throw error;
      }
    },

    async loadAccountInfo() {
      if (!this.accountUrl) {
        this.$toast?.error("لطفا لینک اکانت خود را وارد کنید");
        return;
      }

      try {
        this.loading = true;
        const username = this.parseUsername(this.accountUrl);
        await this.loadAccountInfoByUsername(username);
      } catch (error) {
        console.error("Error loading account info:", error);
      } finally {
        this.loading = false;
      }
    },

    async loadAccountInfoByUsername(username) {
      try {
        this.loading = true;
        const response = await request.get(
          `PublicData/account-info/${username}`
        );
        const data = response.data.result;

        this.userData = {
          username: data.username,
          password: data.password,
          server: data.server,
          expireDate: data.expireDate,
          createDate: data.createDate,
          usedTraffic: data.usedTraffic,
          totalTraffic: data.totalTraffic,
          configString: data.configString,
          selectedProtocol: data.accountType,
          expireDateMessage: data.expireDateMessage,
          trafficMessage: data.trafficMessage,
          isExpired: data.isExpired,
          trafficExpired: data.trafficExpired,
          enable: data.enable,
          enableMessage: data.enableMessage,
        };

        this.guid = username; // Store username as guid for protocol change
        this.accountLoaded = true;
      } catch (error) {
        this.$toast?.error(
          error.response?.data?.message || "خطا در دریافت اطلاعات حساب"
        );
        throw error;
      } finally {
        this.loading = false;
      }
    },

    resetForm() {
      this.accountLoaded = false;
      this.accountUrl = "";
      this.userData = {
        username: "",
        password: "",
        server: "",
        expireDate: "",
        createDate: "",
        usedTraffic: "",
        totalTraffic: "",
        configString: "",
        selectedProtocol: null,
        expireDateMessage: "",
        trafficMessage: "",
        isExpired: false,
        trafficExpired: false,
        enable: false,
        enableMessage: "",
      };
    },

    async fetchProtocols() {
      try {
        const response = await request.get(`PublicData/get-accounType`);
        this.protocols = response.data.result;
      } catch (error) {
        console.error("Error fetching protocols:", error);
      }
    },

    async changeProtocol() {
      // Call server to get config string based on selected protocol
      if (this.userData.selectedProtocol && this.guid) {
        try {
          await request.get(
            `PublicData/ChangeConfig/${this.guid}/${this.userData.selectedProtocol}`
          );
          await this.loadAccountInfoByUsername(this.guid);
          this.$toast?.success("کانفیگ با موفقیت تغییر کرد");
        } catch (error) {
          this.$toast?.error("خطا در تغییر کانفیگ");
        }
      }
    },

    copyConfigString() {
      // Copy configString to clipboard
      navigator.clipboard.writeText(this.userData.configString);
      this.$toast?.success("لینک کانفیگ کپی شد");
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
      try {
        // Fetch video URL from server
        const response = await fetch("/api/videoUrl");
        this.videoUrl = await response.text();
      } catch (error) {
        this.videoUrl =
          "https://s31.uupload.ir/files/toturial/Recorder_27102024_214848.mp4";
      }
    },
  },

  mounted() {
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

.info-row {
  padding: 8px 0;
  border-bottom: 1px solid #f0f0f0;
}

.info-row:last-child {
  border-bottom: none;
}

.download-title {
  font-size: 18px;
  font-weight: bold;
  margin-bottom: 8px;
}
</style>
