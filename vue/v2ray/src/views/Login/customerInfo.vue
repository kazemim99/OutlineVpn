<template>
  <v-container class="pa-4">
    <!-- User Guide Section -->
    <v-row v-if="!accountLoaded">
      <v-col cols="12">
        <v-expansion-panels class="mb-4">
          <v-expansion-panel>
            <v-expansion-panel-header>
              <div class="d-flex align-center">
                <v-icon class="ml-2" color="info">mdi-information</v-icon>
                <strong>راهنمای استفاده از حساب کاربری</strong>
              </div>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <v-card flat>
                <v-card-text>
                  <div class="guide-section">
                    <h4 class="mb-3">🔑 نام کاربری شما:</h4>
                    <p class="mb-2">
                      <strong>Username (نام کاربری):</strong>
                    </p>
                    <p class="text-muted mb-2">مثال: u23564</p>
                    <p class="mb-3">
                      نام کاربری شما که بعد از علامت <code>#</code> در لینک VLESS
                      قرار دارد.
                    </p>

                    <v-divider class="my-4"></v-divider>

                    <h4 class="mb-3">📱 نحوه استفاده:</h4>
                    <ol class="guide-list">
                      <li>
                        <strong>لینک کامل VLESS</strong> یا <strong>نام کاربری</strong>
                        خود را در کادر زیر وارد کنید
                      </li>
                      <li>دکمه "بررسی حساب کاربری" را بزنید</li>
                      <li>
                        اطلاعات حساب شما شامل ترافیک، تاریخ انقضا و وضعیت نمایش
                        داده می‌شود
                      </li>
                      <li>
                        از QR Code برای اتصال سریع در برنامه‌های موبایل استفاده
                        کنید
                      </li>
                    </ol>

                    <v-alert type="info" dense text class="mt-3">
                      <strong>نکته:</strong> می‌توانید یکی از دو روش زیر را انتخاب
                      کنید:
                      <ul class="mt-2 mb-0" style="padding-right: 20px">
                        <li>لینک کامل: vless://...#u23564</li>
                        <li>فقط نام کاربری: u23564</li>
                      </ul>
                    </v-alert>
                  </div>
                </v-card-text>
              </v-card>
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-col>
    </v-row>

    <!-- Saved Account Section -->
    <v-row v-if="!accountLoaded && savedUsername && !showNewAccountInput">
      <v-col cols="12">
        <v-card class="pa-4" color="blue lighten-5">
          <v-card-title>
            <v-icon class="ml-2" color="success">mdi-check-circle</v-icon>
            حساب ذخیره شده
          </v-card-title>
          <v-card-text>
            <v-alert type="success" dense text class="mb-3">
              <strong>نام کاربری ذخیره شده:</strong> {{ savedUsername }}
            </v-alert>
            <v-row>
              <v-col cols="12" sm="6">
                <v-btn
                  color="primary"
                  block
                  large
                  @click="loadSavedAccount"
                  :loading="loading"
                >
                  <v-icon left>mdi-eye</v-icon>
                  مشاهده اطلاعات حساب
                </v-btn>
              </v-col>
              <v-col cols="12" sm="6">
                <v-btn
                  color="warning"
                  block
                  large
                  outlined
                  @click="showNewAccountInput = true"
                >
                  <v-icon left>mdi-account-switch</v-icon>
                  تغییر حساب کاربری
                </v-btn>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Account URL Input Section -->
    <v-row v-if="!accountLoaded && (!savedUsername || showNewAccountInput)">
      <v-col cols="12">
        <v-card class="pa-4">
          <v-card-title>
            <v-icon class="ml-2" color="primary">mdi-account-key</v-icon>
            {{
              showNewAccountInput
                ? "وارد کردن حساب جدید"
                : "اطلاعات حساب کاربری"
            }}
          </v-card-title>
          <v-card-text>
            <v-text-field
              v-model="accountUrl"
              label="لینک VLESS یا نام کاربری خود را وارد کنید"
              placeholder="vless://... یا u23564"
              outlined
              hint="مثال لینک کامل: vless://...#u23564 یا مثال نام کاربری: u23564"
              persistent-hint
            ></v-text-field>
            <v-checkbox
              v-model="rememberAccount"
              label="ذخیره نام کاربری برای دفعات بعد"
              color="primary"
              class="mt-2"
            ></v-checkbox>
            <v-row>
              <v-col cols="12" :sm="showNewAccountInput ? 6 : 12">
                <v-btn
                  color="primary"
                  class="mt-3"
                  block
                  large
                  @click="loadAccountInfo"
                  :loading="loading"
                >
                  <v-icon left>mdi-magnify</v-icon>
                  بررسی حساب کاربری
                </v-btn>
              </v-col>
              <v-col v-if="showNewAccountInput" cols="12" sm="6">
                <v-btn
                  color="grey"
                  class="mt-3"
                  block
                  large
                  outlined
                  @click="cancelNewAccount"
                >
                  <v-icon left>mdi-close</v-icon>
                  انصراف
                </v-btn>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Account Information Display -->
    <v-row v-if="accountLoaded">
      <!-- Back Button and Actions -->
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center flex-wrap">
          <v-btn text color="primary" @click="resetForm">
            <v-icon left>mdi-arrow-right</v-icon>
            بازگشت
          </v-btn>
          <v-btn
            v-if="savedUsername"
            text
            color="error"
            @click="forgetAccount"
          >
            <v-icon left>mdi-delete</v-icon>
            حذف حساب ذخیره شده
          </v-btn>
        </div>
      </v-col>

      <!-- Account Status Card -->
      <v-col cols="12" md="4">
        <v-card class="pa-3">
          <v-card-title class="text-h6">
            <v-icon class="ml-2" color="primary">mdi-account-circle</v-icon>
            اطلاعات حساب
          </v-card-title>
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

  </v-container>
</template>

<script>
import request from "@/utils/request";

export default {
  data() {
    return {
      protocols: [], // Protocol options
      configString: "", // Config string received from server
      guid: null,
      accountUrl: "", // Input for account URL
      accountLoaded: false, // Flag to show account info
      loading: false, // Loading state
      savedUsername: "", // Saved username from localStorage
      showNewAccountInput: false, // Show new account input form
      rememberAccount: true, // Remember account checkbox
      userData: {
        username: "",
        guid: "",
        password: "",
        server: "",
        expireDate: "",
        createDate: "",
        usedTraffic: "",
        totalTraffic: "",
        configString: "",
        configUrl: "",
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

    // Load saved username from localStorage
    this.loadSavedUsername();

    // Check if there's a username in the route
    const username = this.$route.query.username;
    if (username) {
      this.loadAccountInfoByUsername(username);
    }
  },

  methods: {
    // LocalStorage methods
    loadSavedUsername() {
      try {
        const saved = localStorage.getItem("v2ray_username");
        if (saved) {
          this.savedUsername = saved;
        }
      } catch (error) {
        console.error("Error loading saved username:", error);
      }
    },

    saveUsername(username) {
      try {
        localStorage.setItem("v2ray_username", username);
        this.savedUsername = username;
      } catch (error) {
        console.error("Error saving username:", error);
        this.$toast?.error("خطا در ذخیره نام کاربری");
      }
    },

    clearSavedUsername() {
      try {
        localStorage.removeItem("v2ray_username");
        this.savedUsername = "";
      } catch (error) {
        console.error("Error clearing saved username:", error);
      }
    },

    loadSavedAccount() {
      if (this.savedUsername) {
        this.loadAccountInfoByUsername(this.savedUsername);
      }
    },

    cancelNewAccount() {
      this.showNewAccountInput = false;
      this.accountUrl = "";
    },

    forgetAccount() {
      if (confirm("آیا مطمئن هستید که می‌خواهید حساب ذخیره شده را حذف کنید؟")) {
        this.clearSavedUsername();
        this.resetForm();
        this.$toast?.success("حساب ذخیره شده حذف شد");
      }
    },

    parseUsername(input) {
      // Handle both full VLESS URL and plain username
      // The backend will extract the username if it's a full URL
      try {
        const trimmedInput = input.trim();
        if (!trimmedInput) {
          throw new Error("لطفا لینک یا نام کاربری را وارد کنید");
        }

        // If it's a VLESS URL, we can optionally extract username here
        // But the backend also handles this, so we can pass it as-is
        if (trimmedInput.startsWith("vless://")) {
          // Validate that it has the # symbol
          const hashIndex = trimmedInput.indexOf("#");
          if (hashIndex === -1) {
            throw new Error("نام کاربری در لینک یافت نشد (بعد از علامت #)");
          }
          const username = trimmedInput.substring(hashIndex + 1).trim();
          if (!username) {
            throw new Error("نام کاربری خالی است");
          }
          return username;
        }

        // If it's just a username, return it as-is
        return trimmedInput;
      } catch (error) {
        this.$toast?.error(error.message || "فرمت ورودی اشتباه است");
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

        // Save username if remember checkbox is checked
        if (this.rememberAccount) {
          this.saveUsername(username);
        }

        // Reset the new account input flag
        this.showNewAccountInput = false;
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
          guid: data.guid,
          password: data.password,
          server: data.server,
          expireDate: data.expireDate,
          createDate: data.createDate,
          usedTraffic: data.usedTraffic,
          totalTraffic: data.totalTraffic,
          configString: data.configString || data.configUrl,
          configUrl: data.configUrl,
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
        guid: "",
        password: "",
        server: "",
        expireDate: "",
        createDate: "",
        usedTraffic: "",
        totalTraffic: "",
        configString: "",
        configUrl: "",
        selectedProtocol: null,
        expireDateMessage: "",
        trafficMessage: "",
        isExpired: false,
        trafficExpired: false,
        enable: false,
        enableMessage: "",
      };
    },

    copyToClipboard(text, label = "متن") {
      if (!text) {
        this.$toast?.error("محتوایی برای کپی وجود ندارد");
        return;
      }
      navigator.clipboard
        .writeText(text)
        .then(() => {
          this.$toast?.success(`${label} با موفقیت کپی شد`);
        })
        .catch(() => {
          this.$toast?.error("خطا در کپی کردن");
        });
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

/* Guide Section Styles */
.guide-section {
  font-family: "Vazir", "Tahoma", sans-serif;
  direction: rtl;
  text-align: right;
}

.guide-section h3 {
  color: #1976d2;
  font-size: 18px;
  font-weight: bold;
}

.guide-section h4 {
  color: #424242;
  font-size: 16px;
  font-weight: bold;
  margin-top: 12px;
}

.guide-section p {
  color: #616161;
  line-height: 1.8;
}

.guide-section code {
  font-family: "Courier New", monospace;
  font-size: 12px;
  word-break: break-all;
  direction: ltr;
  text-align: left;
}

.guide-list {
  padding-right: 20px;
  margin-bottom: 12px;
}

.guide-list li {
  margin-bottom: 12px;
  line-height: 1.8;
}

.guide-list .text-muted {
  color: #757575;
  font-size: 13px;
  display: block;
  margin-top: 4px;
  direction: ltr;
  text-align: left;
}

/* GUID Text Styles */
.guid-text {
  font-size: 11px;
  background: #f5f5f5;
  padding: 4px 8px;
  border-radius: 4px;
  font-family: "Courier New", monospace;
  word-break: break-all;
  flex: 1;
  direction: ltr;
  text-align: left;
}

/* Config URL Container */
.config-url-container {
  position: relative;
}

.config-textarea {
  font-family: "Courier New", monospace;
  font-size: 12px;
  direction: ltr;
  text-align: left;
}

/* Download Section */
.download-section {
  text-align: center;
}

.download-section h4 {
  font-size: 16px;
  font-weight: bold;
  margin-bottom: 12px;
}
</style>
