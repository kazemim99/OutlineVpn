<template>
  <div class="small">
    <v-card class="mx-auto" max-width="400" outlined>
      <v-card-title>اطلاعات سرور</v-card-title>

      <v-card-subtitle class="pb-0">
        این اطلاعات در قسمت آموزش که توضیح داده شده باید وارد نمایید
        <br />
        در صورتی که قبلا اطلاعات سرور را دریافت کرده اید دکمه زیر ر ابزنید
        <br />
        <Userpass ref="userPassCom" />
      </v-card-subtitle>

      <v-list-item three-line class="mt-4">
        <v-list-item-content>
          <v-row>
            <v-col md="6" sm="12">
              <div class="mb-2">تاریخ اعتبار :</div>
            </v-col>
            <v-col md="6" sm="12">
              <div class="mb-2">{{ this.userKeyDetails.expireDate }}</div>
            </v-col>
          </v-row>

          <v-row>
            <v-col md="6" sm="12">
              <div class="mb-2">ترافیک مصرف شده :</div>
            </v-col>
            <v-col md="6" sm="12">
              <div class="mb-2">{{ this.userKeyDetails.usedTraffic }}</div>
            </v-col>
          </v-row>

          <v-row>
            <v-col md="6" sm="12">
              <div class="mb-2">نام کاربری (Username) :</div>
            </v-col>
            <v-col md="6" sm="12">
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
            <v-col md="6" sm="12">
              <div class="mb-2">رمز عبور (Password) :</div>
            </v-col>
            <v-col md="6" sm="12">
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
            <!-- <v-col md="6" sm="6">
              <v-btn
                href="https://blog.iranv2ray.com/%d8%a2%d9%85%d9%88%d8%b2%d8%b4-%d8%a7%d8%aa%d8%b5%d8%a7%d9%84/"
                >آموزش استفاده</v-btn
              >
            </v-col> -->
          </v-row>
          <div class="text-center mt-10">
            <v-btn
              v-if="userKeyDetails.userName == null"
              rounded
              color="primary"
              :loading="loading"
              @click="getKey()"
              dark
            >
              دریافت سرور 2 ساعته تستی
            </v-btn>
            <v-row v-else>
              <v-col cols="6"> </v-col>
              <v-col cols="2"> </v-col>
            </v-row>
            <v-spacer></v-spacer>
          </div>
        </v-list-item-content>
      </v-list-item>
      <v-card-actions>
        <AddOrder ref="addOrderCom" />
        <AddProblmeReport ref="addProblemReportCom" />
      </v-card-actions>
      پشتیبانی تلگرام :
      <v-col md="6" sm="6">
        <a
          large
          target="”_blank"
          style="margin: auto"
          text
          color="success"
          href="https://t.me/+GRPLkWQHXD5jNGZk"
          >ورود به کانال</a
        >
      </v-col>

      <v-card-subtitle class="pb-0">
        در صورت عدم رضایت بعد از ده روز در قسمت گزارش قطعی گزینه بازگشت وجه را
        بزنید تا مبلغ به حسابی که با آن پرداخت کرده ایید واریز شود
      </v-card-subtitle>
    </v-card>
    <v-row>
      <v-row>
        <v-col
          v-for="card in cards"
          class="mt-10 p-10"
          :key="card.id"
          sm="12"
          md="6"
        >
          <v-hover v-slot="{ hover }">
            <v-card
              :href="card.url"
              :elevation="hover ? 16 : 2"
              :class="{ 'on-hover': hover }"
              style="background-color: #00b894; cursor: pointer"
            >
              <v-img :src="card.image" width="350" contain> </v-img>
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
    </v-row>
    <div></div>
  </div>
</template>

<script>
import request from "@/utils/request";
import AddProblmeReport from "@/components/common/ProblemReport.vue";
import AddOrder from "@/components/Home/Order.vue";
import Userpass from "@/components/Home/UserPass.vue";
import Vue from "vue";

export default {
  components: {
    AddProblmeReport,
    AddOrder,
    Userpass,
  },
  data() {
    return {
      cards: [
        {
          fileName: "",
          name: "آموزش استفاده اندروید و آیفون",
          url: "/dashboard/phone-toturial",
          image: "/phone.jpg",
        },

        {
          fileName: "netmode.apk",
          name: "آموزش استفاده ویندوز",
          url: "/dashboard/windows-toturial",
          image: "/windows.jpg",
        },
      ],
      loading: false,
      loading1: false,
      keys: [],
      down: "مشاهده",
      count: 1,
      userKeyDetails: {
        password: "",
        expireDate: null,
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
          const data = response.data.message;
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
          const data = response.data.result;
          this.loading = false;
          Vue.swal(
            "تبریک",
            "اکانت تست دو ساعته برای شما فعال شد برای خرید اکانت یک ماه دکمه تمدید در صفحه اصلی را بزنید",
            "info"
          );

          this.getUserKeyDetails();
        })
        .catch(() => {
          this.loading = false;
        });
    },
    getUserKeyDetails() {
      request.get(`/sshKey/user-key-details`).then((response) => {
        const data = response.data.result;
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
