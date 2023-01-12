<template>
  <div class="small">
    <v-row>
      <v-col md="6" sm="3">
        <div class="grey--text mb-2">تاریخ اعتبار</div>
      </v-col>
      <v-col md="6" sm="6">
        <div class="mb-2">{{ this.userKeyDetails.expireTime }}</div>
      </v-col>
    </v-row>
    <v-row>
      <v-col md="6" sm="3">
        <div class="grey--text mb-2">کد اتصال</div>
      </v-col>
      <v-col md="6" sm="6">
        <v-row>
          <div class="mb-2">{{ this.userKeyDetails.key }}</div>
          <v-btn
            rounded
            color="success"
            v-if="this.userKeyDetails.key"
            @click="copyToClipBoard(userKeyDetails.key)"
            dark
            >کپی</v-btn
          >
        </v-row>
      </v-col>
    </v-row>
    <!-- <v-row>
      <v-col cols="6">
        <div class="grey--text mb-2">ترافیک باقی مانده</div>
      </v-col>
      <v-col cols="6">
        <div class="mb-2">{{ this.raminingTraffic }} گیگا بایت</div>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="6">
        <div class="grey--text mb-2">ترافیک خریداری شده</div>
      </v-col>
      <v-col cols="6">
        <div class="mb-2">{{ this.initTraffic }} گیگا بایت</div>
      </v-col>
    </v-row> -->
    <div class="text-center mt-10">
      <v-btn
        rounded
        color="primary"
        :loading="loading"
        @click="!userKeyDetails.freeAccount ? getKey() : buyKey()"
        dark
      >
        {{ !userKeyDetails.freeAccount ? "دریافت VPN رایگان" : "تمدید" }}
      </v-btn>
    </div>
    <v-row>
        <v-col v-for="card in cards" class="mt-10 p-10" :key="card.id" cols="12" sm="4" md="3">
          <v-hover v-slot="{ hover }">
            <v-card
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
  </div>
</template>

<script>
import request from "@/utils/request";
import { UserModule } from "@/store/modules/user";

export default {
  data() {
    return {
      cards: [
        {
          name: "دانلود مستقیم برنامه",
          url: "https://github.com/2dust/v2rayNG/releases/download/1.7.34/v2rayNG_1.7.34.apk",
          image: require("@/assets/images/apk.png"),
        },
        {
          name: "دانلود گوگل استور",
          url: "https://play.google.com/store/apps/details?id=com.v2ray.ang&hl=en&gl=US",
          image: require("@/assets/images/google.png"),
        },
        {
          name: "دانلود اپ استور",
          url: "https://apps.apple.com/us/app/fair-vpn/id1533873488",
          image: require("@/assets/images/appstore.png"),
        },
        // {
        //   name: "(بزودی) ویندوز",
        //   url: "",
        //   image: require("@/assets/images/appstore.png"),
        // },
        // {
        //   name: "مک (بزودی)",
        //   url: "",
        //   image: require("@/assets/images/appstore.png"),
        // },
        // {
        //   name: "لینوکس (بزودی)",
        //   url: "",
        //   image: require("@/assets/images/appstore.png"),
        // },
      ],
      loading: false,
      keys: [],
      count: 1,
      userKeyDetails: {
        key: "",
        freeAccount: false,
        up: 0,
        down: 0,
        total: 0,
        expireTime: null,
      },
    };
  },
  mounted() {
    this.getUserKeyDetails();
  },
  methods: {
    buyKey() {
      alert("خرید");
    },
    copyToClipBoard(textToCopy) {
      navigator.clipboard.writeText(textToCopy);
    },
    getKey() {
      this.loading = true;
      request
        .get(`/v2Key/generateKey/${this.count}`)
        .then((response) => {
          var data = response.data.result;
          this.keys = data;
          this.loading = false;
          this.getUserKeyDetails();
        })
        .catch(() => {
          this.loading = false;
        });
    },
    getUserKeyDetails() {
      request.get(`/v2Key/user-key-details`).then((response) => {
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
