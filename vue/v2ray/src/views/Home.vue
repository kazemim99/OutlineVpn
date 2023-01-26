<template>
  <div class="small">
    <v-col cols="12">
      <AddProblmeReport ref="addProblemReportCom" />
    </v-col>
    <v-spacer></v-spacer>
    <v-row>
      <v-col md="6" sm="3">
        <div class="grey--text mb-2">تاریخ اعتبار :</div>
      </v-col>
      <v-col md="6" sm="6">
        <div class="mb-2">{{ this.userKeyDetails.expireTime }}</div>
      </v-col>
    </v-row>

    <v-row>
      <v-col md="6" sm="3">
        <div class="grey--text mb-2">آدرس سرور (Hostname/IP) :</div>
      </v-col>
      <v-col md="6" sm="6">
        <div class="mb-2">{{ this.userKeyDetails.hostName }}</div>
      </v-col>
    </v-row>

    <v-row>
      <v-col md="6" sm="3">
        <div class="grey--text mb-2">پورت (Port) :</div>
      </v-col>
      <v-col md="6" sm="6">
        <div class="mb-2">{{ this.userKeyDetails.port }}</div>
      </v-col>
    </v-row>

    <v-row>
      <v-col md="6" sm="6">
        <div class="grey--text mb-2">نام کاربری (Username) :</div>
      </v-col>
      <v-col md="6" sm="6">
        <v-row>
          <div>{{ this.userKeyDetails.userName }}</div>
          <v-btn
            class="mr-5 mb-2"
            small
            rounded
            color="success"
            v-if="this.userKeyDetails.userName"
            @click="copyToClipBoard(userKeyDetails.userName)"
            dark
            >کپی</v-btn
          >
        </v-row>
      </v-col>
    </v-row>
    <v-row>
      <v-col md="6" sm="6">
        <div class="grey--text mb-2">رمز عبور (Password) :</div>
      </v-col>
      <v-col md="6" sm="6">
        <v-row>
          <div>{{ this.userKeyDetails.password }}</div>
          <v-btn
            class="mr-5 mb-2"
            rounded
            small
            color="success"
            v-if="this.userKeyDetails.userName"
            @click="copyToClipBoard(userKeyDetails.password)"
            dark
            >کپی</v-btn
          >
        </v-row>
      </v-col>
    </v-row>
    <div class="text-center mt-10">
      <v-btn
        v-if="!userKeyDetails.userName"
        rounded
        color="primary"
        :loading="loading"
        @click="getKey()"
        dark
      >
        "دریافت سرور رایگان"
      </v-btn>
      <AddOrder ref="addOrderCom" />
    </div>
    <v-row>
      <v-col
        v-for="card in cards"
        class="mt-10 p-10"
        :key="card.id"
        cols="12"
        sm="4"
        md="4"
      >
        <v-hover v-slot="{ hover }">
          <v-card
            target="_blank"
            :download="card.fileName"
            :href="card.url"
            :elevation="hover ? 16 : 2"
            :class="{ 'on-hover': hover }"
            style="background-color: #00b894; cursor: pointer"
          >
            <v-img :src="card.image" height="150"> </v-img>
            <v-card-actions>
              <span
                style="text-align: center"
                class="text-h6 white--text d-inline-block"
                v-text="card.name"
              ></span>
            </v-card-actions>
          </v-card>
        </v-hover>
      </v-col>
    </v-row>
    <div>
      <br /><br />

      <Toturial />
    </div>
  </div>
</template>

<script>
import request from "@/utils/request";
import AddProblmeReport from "@/components/common/ProblemReport.vue";
import Toturial from "@/components/Home/Toturial.vue";
import AddOrder from "@/components/Home/Order.vue";

export default {
  components: {
    AddProblmeReport,
    Toturial,
    AddOrder,
  },
  data() {
    return {
      cards: [
        {
          fileName: "",
          name: " دانلود مستقیم برای اندروید (NapsternetV)",
          url: "/files/NapsternetV.apk",
          image: require("@/assets/images/apk.png"),
        },
        {
          fileName: "netmode.apk",
          name: "  (NapsternetV) گوگل استور",
          url: "https://play.google.com/store/apps/details?id=com.napsternetlabs.napsternetv&hl=en&gl=US",
          image: require("@/assets/images/google.png"),
        },
        {
          fileName: "netmode.apk",

          name: "دانلود اپ استور (NapsternetV)",
          url: "https://apps.apple.com/us/app/napsternetv/id1629465476",
          image: require("@/assets/images/appstore.png"),
        },
        {
          fileName: "netmode.apk",
          name: "دانلود مستقیم (Netmode Syna)",
          url: "/files/NetModSyna-VPNClient_1.11.3.apk",
          image: require("@/assets/images/apk.png"),
        },
        {
          fileName: "netmode.exe",
          name: "ویندوز",
          url: "/files/NetMod_x86(Latest).exe",
          image: require("@/assets/images/windows.png"),
        },
        {
          name: "مک (بزودی)",
          url: "",
          image: require("@/assets/images/appstore.png"),
        },
        {
          name: "لینوکس (بزودی)",
          url: "",
          image: require("@/assets/images/appstore.png"),
        },
      ],
      loading: false,
      loading1: false,
      keys: [],
      down: "مشاهده",
      count: 1,
      userKeyDetails: {
        expireTime: null,
      },
    };
  },
  mounted() {
    this.getUserKeyDetails();
  },
  methods: {
    buyKey() {
      this.$refs.addUserCom.dialog = true;
    },
    getTraffic() {
      this.loading1 = true;
      request
        .get(`/v2Key/getUsedTraffic`)
        .then((response) => {
          var data = response.data.message;
          debugger;
          this.loading1 = false;
          this.down = data;
        })
        .catch(() => {
          this.loading = false;
          this.loading1 = false;
        });
    },
    copyToClipBoard(textToCopy) {
      navigator.clipboard
        .writeText(textToCopy)
        .then(() => {
          alert("کپی شد");
        })
        .catch(() => {
          alert("خطا در کپی");
        });
    },
    getKey() {
      this.loading = true;
      request
        .get(`/sshkey/create-test-ssh`)
        .then((response) => {
          var data = response.data.result;
          this.loading = false;
          this.getUserKeyDetails();
        })
        .catch(() => {
          this.loading = false;
        });
    },
    getUserKeyDetails() {
      request.get(`/sshKey/user-key-details`).then((response) => {
        var data = response.data.result;
        this.userKeyDetails = data;
      });
    },
  },
};
</script>

<style>
.small {
  max-width: 600px;
  margin: 150px auto;
}
.v-dialog {
  font-family: arial, sans-serif;
}

table {
  font-family: arial, sans-serif;
  border-collapse: collapse;
  width: 100%;
}

td,
th {
  border: 1px solid #dddddd;
  text-align: left;
  padding: 8px;
}

tr:nth-child(even) {
  background-color: #dddddd;
}
</style>
