<template>
  <div class="small">
    <v-row justify="right">
      <v-col md="12">
        <h1>آموزش استفاده اندروید و آیفون :</h1>
        <h3>ابتدا برنامه را با توجه به سیستم عامل گوشی خود دانلود کنید</h3>
      </v-col>
    </v-row>
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
            download
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

      <PhoneToturial />
    </div>
  </div>
</template>

<script>
import PhoneToturial from "@/components/Home/PhoneToturial.vue";

export default {
  components: {
    PhoneToturial,
  },
  data() {
    return {
      cards: [
        {
          fileName: "",
          name: " دانلود مستقیم برای اندروید (NapsternetV)",
          url: "https://fastp.playstoreapi.com/download?token=MTAxOTM5NzQzZTg5ZjExZDNlMzY1MjdhYWEzZjI1OTM0ZDZjNDYzY2ExNzFhMmEwZWE2OTgzYjM0NzFk&a=y",
          image: require("@/assets/images/apk.png"),
        },
        {
          fileName: "netmode.apk",
          name: " اندروید (Netmode syna) ",
          url: "https://googleapis.apk.services/apk/4409919da6b7e5ebd84e8eedb96f324824b8aa642ab2cf3c602a09ea8f3e40f8846edb48510ff7aa72d0c6bcdb95214f14c0c6f68a0c6ed704595f801115b37cdf6ca5202290f689b666e9be122b307657899683aad126864529cdce3da9bfb30e0266372bf2796269f55c19b4a790ccfadbdf376c6065dd55ffddc96ec091da76477dea977a76a1478a0f0c907b5aecd2d038935fdc726c320e2d2c9224b4ab651960b4879056bb8d7daeedc6fd36d2f83018034bc838d70cc8365ba409eb0ab3406f8b",
          image: require("@/assets/images/google.png"),
        },
        {
          fileName: "netmode.apk",
          name: "گوگل استور (NapsternetV) ",
          url: "https://play.google.com/store/apps/details?id=com.napsternetlabs.napsternetv&hl=en&gl=US",
          image: require("@/assets/images/google.png"),
        },

        {
          fileName: "netmode.apk",

          name: "دانلود اپ استور (NapsternetV)",
          url: "https://apps.apple.com/us/app/napsternetv/id1629465476",
          image: require("@/assets/images/appstore.png"),
        },
      ],
      loading: false,
      loading1: false,
      keys: [],
      down: "مشاهده",
      count: 1,
    };
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
