<template>
  <div class="small">
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
          url: "/files/NapsternetV.apk",
          image: require("@/assets/images/apk.png"),
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
