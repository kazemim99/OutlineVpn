<template>
  <v-row>
    <v-col cols="4">
      <v-select
        v-model="selectedComplexId"
        :items="complexes"
        item-value="id"
        item-text="text"
        label="مجموعه"
        @change="getSubComplexes"
        solo
      ></v-select>
    </v-col>

    <v-col cols="4">
      <v-select
        v-model="selectedSubComplexId"
        :items="subComplexes"
        item-value="id"
        item-text="text"
        label="زیر مجموعه"
        @change="getDevices(selectedSubComplexId)"
        solo
      ></v-select>
    </v-col>
    <v-col cols="4">
      <v-select
        v-model="deviceId"
        :items="devices"
        item-value="id"
        item-text="text"
        label="دستگاه"
        solo
      ></v-select>
    </v-col>
  </v-row>
</template>

<script>
import request from "@/utils/request";

export default {
  name: "DeviceFilterDropDown",
  data() {
    return {
      selectedComplexId: null,
      selectedSubComplexId: null,
      deviceId: null,
      subComplexes: [],
      complexes: [],
      devices: [],
    };
  },
  created() {
    this.getComplexes();
  },
  methods: {
    async getComplexes() {
      await request.get(`/publicData/main-complexes`).then((response) => {
        const data = response.data.result;
        this.complexes = data;
        this.complexes.unshift({ id: null, text: "انتخاب..." });
      });
    },
    async getSubComplexes() {
      if (this.selectedComplexId) {
        await request
          .get(`/publicData/sub-complexes/${this.selectedComplexId}`)
          .then((response) => {
            const data = response.data.result;
            this.subComplexes = data;
            this.subComplexes.unshift({ id: null, text: "انتخاب..." });
            this.getDevices(this.selectedComplexId);
          });
      }
    },
    async getDevices(complexId) {
      if (complexId) {
        await request
          .get(`/publicData/main-complexes-devices/${complexId}`)
          .then((response) => {
            const data = response.data.result;
            this.devices = data;
            this.devices.unshift({ id: null, text: "انتخاب..." });
          });
      }
    },
  },
};
</script>

<style scoped>
/* Modal Content (image) */
</style>
