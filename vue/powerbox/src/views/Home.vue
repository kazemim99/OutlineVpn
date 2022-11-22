<template>
  <div class="small">
    <v-row>
      <v-col cols="9">
        <v-text-field
          v-model="phone"
          label="شماره"
          prepend-icon="mdi-search"
        ></v-text-field>
      </v-col>
      <v-col cols="2" style="margin-top: 12px">
        <v-btn depressed @click="search()" color="primary"> جستجو </v-btn>
      </v-col>
    </v-row>

    <v-data-table
      v-if="hasData"
      :headers="headers"
      :items="logs"
      :loading="loading"
      class="elevation-1"
    ></v-data-table>

  </div>
</template>

<script>
import request from "@/utils/request";

export default {
 
  data() {
    return {
      hasData: false,
      phone: null,
      datacollection: null,
      cableType: {},
      logs: [],
      headers: [
        { text: "مجموعه", value: "complex", sortable: false },
        { text: "دستگاه", value: "device", sortable: false },
        { text: "شماره", value: "phone", sortable: false },
        { text: "ساعت", value: "time", sortable: false },
        { text: "آخرین وضعیت", value: "lastSate", sortable: false },
      ],
    };
  },
 
  methods: {
    search() {
      request.get(`/DeviceManagement/search/${this.phone}`).then((response) => {
        var data = response.data.result;
        this.hasData = true;
        this.logs = data;
      });
    },
  

    getRandomInt() {
      return Math.floor(Math.random() * (50 - 5 + 1)) + 5;
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
