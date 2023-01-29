<template>
  <v-app>
    <router-view />
  </v-app>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import VueMeta from "vue-meta";

@Component({
  name: "App",

  metaInfo: {
    title: "ایران وی تو ری  | فروش فیلتر شکن اندروید , ایفون , ویندوز و لینوکس",
    htmlAttrs: {
      lang: "fa-IR",
    },
    meta: [
      { charset: 'utf-8' },
      { name: 'description', content: 'فروش فیلتر شکن با سه روز مهلت تست و پشتیبانی همه روزه بدون قطعی و پر سرعت برای اندروید , ایفون, ویندوز و لینوکس' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { property: "og:site_name", content: "iranv2ray.com" },
      {
        property: "og:description",
        content:
          "فروش فیلتر شکن پر سرعت برای اندرودید , ایفون , لینوکس , اندروید , ویندوز و کامپیوتر",
      },
      { property: "og:type", content: "site" },
      { property: "og:url", content: "https://iranv2ray.com" },
      { property: "og:image", content: "<%= BASE_URL %>siteimage.png" },
    ]
  },
})
export default class extends Vue {}
</script>
