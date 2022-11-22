<template>
  <v-row>
    <v-col cols="3">
      <v-menu
        v-model="menu1"
        :close-on-content-click="false"
        :nudge-right="40"
        transition="scale-transition"
        offset-y
        min-width="auto"
      >
        <template v-slot:activator="{ on, attrs }">
          <v-text-field
            clearable
            @click:clear="clearFrom()"
            v-model="formattedDate"
            label="از"
            prepend-icon="mdi-calendar"
            readonly
            v-bind="attrs"
            v-on="on"
          ></v-text-field>
        </template>
        <v-date-picker
          :first-day-of-week="0"
          locale="fa-ir"
          v-model="from"
          @input="menu1 = false"
        ></v-date-picker>
      </v-menu>
    </v-col>

    <v-col cols="3">
      <v-menu
        v-model="menu2"
        :close-on-content-click="false"
        :nudge-right="40"
        transition="scale-transition"
        offset-y
        min-width="auto"
      >
        <template v-slot:activator="{ on, attrs }">
          <v-text-field
            clearable
            @click:clear="clearTo()"
            v-model="formattedDate1"
            label="تا"
            prepend-icon="mdi-calendar"
            readonly
            v-bind="attrs"
            v-on="on"
          ></v-text-field>
        </template>
        <v-date-picker
          :first-day-of-week="0"
          locale="fa-ir"
          v-model="to"
          @input="menu2 = false"
        ></v-date-picker>
      </v-menu>
    </v-col>
  </v-row>
</template>

<script>
import request from "@/utils/request";

export default {
  name: "DeviceFilterDropDown",
  data() {
    return {
      menu1: "",
      menu2: "",
      from: "",
      to: "",
    };
  },
  computed: {
    formattedDate: function () {
      // !! format the date based on this.currentLocale !!
      let formattedDate = "";
      let options = {
        weekday: "short",
        year: "numeric",
        month: "2-digit",
        day: "numeric",
      };
      if (this.from) {
        formattedDate = new Date(this.from).toLocaleDateString("fa", options);
      }
      return formattedDate;
    },
    formattedDate1: function () {
      // !! format the date based on this.currentLocale !!
      let formattedDate = "";
      let options = {
        weekday: "short",
        year: "numeric",
        month: "2-digit",
        day: "numeric",
      };
      if (this.to) {
        formattedDate = new Date(this.to).toLocaleDateString("fa", options);
      }
      return formattedDate;
    },
  },
  methods: {
    clearFrom() {
      this.from = null;
    },
    clearTo() {
      this.to = null;
    },
  },
};
</script>

<style scoped>
/* Modal Content (image) */
</style>
