<template>
  <div class="small">
    <v-row>
      <v-col cols="6">
        <div class="grey--text mb-2">تاریخ اعتبار</div>
      </v-col>
      <v-col cols="6">
        <div class="mb-2">{{ this.userKeyDetails.expireTime }}</div>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="6">
        <div class="grey--text mb-2">کد اتصال</div>
      </v-col>
      <v-col cols="6">
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
  </div>
</template>

<script>
import request from "@/utils/request";
import { UserModule } from "@/store/modules/user";

export default {
  data() {
    return {
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
