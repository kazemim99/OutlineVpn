<template>
  <div id="myModal" class="modal" style="z-index: 99999">
    <img class="modal-content" width="450px" height="450px" id="img01" />
    <span
      class="close"
      @click="closeModal"
      style="
        color: red;
        font-size: 32px;
        cursor: pointer;
        position: absolute;
        left: 19px;
        right: 0px;
        bottom: 44px;
        bottom: -18px;
        top: -60px;
      "
      >&times;</span
    >

    <span
      @click="rotate"
      style="
        font-size: 32px;
        cursor: pointer;
        position: absolute;
        left: 5px;
        right: 26px;
        bottom: -18px;
        top: -63px;
      "
    >
      <!-- <v-icon>mdi-rotate-right</v-icon> -->
    </span>
  </div>
</template>

<script>
export default {
  name: "Rotate",
  data() {
    return {
      deg: 0,
    };
  },
  methods: {
    rotate() {
      this.deg += 90;
      const imgs = document.getElementById("img01");
      imgs.style.transform = `rotate(${this.deg}deg)`;
    },
    closeModal() {
      const modal = document.getElementById("myModal");
      modal.style.display = "none";
    },
    openModel(image) {
      const modal = document.getElementById("myModal");
      const imgs = document.getElementById("img01");
      imgs.src = image;
      modal.style.display = "block";
    },
  },
};
</script>

<style scoped>
#myModal {
  display: none;
  position: fixed;
  width: 400px;
  height: 400px;
  margin: 5% auto; /* Will not center vertically and won't work in IE6/7. */
  left: 0;
  right: 0;
  top: 10%;
}
/* Modal Content (image) */
</style>
