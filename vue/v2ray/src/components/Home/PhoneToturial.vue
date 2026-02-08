<template>
  <v-container fluid>
    <v-row>
      <v-col
        v-for="(image, index) in images"
        :key="index"
        :cols="cols"
        :sm="12"
      >
        <v-img
          :src="image.src"
          :alt="image.alt"
          width="350"
          contain
          height="600"
        ></v-img>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  data() {
    return {
      images: [
        {
          src: require("@/assets/images//toturial/NapsternetV/1.jpg"),
          alt: "Image 1",
        },
        {
          src: require("@/assets/images//toturial/NapsternetV/2.jpg"),
          alt: "Image 1",
        },
        {
          src: require("@/assets/images//toturial/NapsternetV/3.jpg"),
          alt: "Image 1",
        },
        {
          src: require("@/assets/images//toturial/NapsternetV/4.jpg"),
          alt: "Image 1",
        },
        {
          src: require("@/assets/images//toturial/NapsternetV/5.jpg"),
          alt: "Image 1",
        },
        {
          src: require("@/assets/images//toturial/NapsternetV/6.jpg"),
          alt: "Image 1",
        },
        {
          src: require("@/assets/images//toturial/NapsternetV/7.jpg"),
          alt: "Image 1",
        },
        {
          src: require("@/assets/images//toturial/NapsternetV/8.jpg"),
          alt: "Image 1",
        },
        {
          src: require("@/assets/images//toturial/NapsternetV/9.jpg"),
          alt: "Image 1",
        },
      ],
      cols: 4,
    };
  },
  mounted() {
    if (this.$vuetify.breakpoint.width < 600) {
      this.cols = 12;
    }
  },
};
</script>
