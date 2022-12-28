<template>
  <div class="small">
    <v-row>
      <v-col cols="6">
        <div class="grey--text mb-2">ترافیک مصرف شده</div>
      </v-col>
      <v-col cols="6">
        <div class="mb-2">{{ this.consumedTraffic }} گیگا بایت</div>
      </v-col>
    </v-row>
    <v-row>
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
    </v-row>
    <div class="text-center mt-10">
      <v-btn to="/plans" rounded color="primary" @click="getKey()" dark>
        {{ this.feeAccount ? "دریافت کلید رایگان" : "خرید ترافیک" }}
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
      keys:[],
      count:1,
      userKeyDetails: {
        freeAccount: false,
        up: 0,
        down: 0,
        total: 0,
        expireTime: null,
      },
    };
  },
  mounted() {
    this.getConsumedTraffic();
  },
  methods: {
    getKey() {
      request.get(`/v2Key/generateKey/${this.count}`).then((response) => {
        var data = response.data.result;
        this.keys = data;
      });
    },
    getConsumedTraffic() {
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
