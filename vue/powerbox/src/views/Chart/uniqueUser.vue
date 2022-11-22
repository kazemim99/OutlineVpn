<template>
  <v-container>
    <deviceFilterDropDown ref="complex" />
    <calenderSelect ref="dateRef" />

    <v-row>
      <v-btn :loading="loading" color="blue darken-1" @click="getCableType()"
        >مشاهد</v-btn
      >
    </v-row>
    <v-col cols="6">
      <table style="margin-top: 80px">
        <tr>
          <th>تاریخ</th>
          <th>T1</th>
          <th>T2</th>
          <th>T4</th>
          <th>T6</th>
          <th>total unique user per day</th>
          <th>avg unique user per day</th>
        </tr>
        <tr v-for="item in items" :key="item.Date">
          <td>{{ item.Date }}</td>
          <td>{{ item.T1 }}</td>
          <td>{{ item.T2 }}</td>
          <td>{{ item.T4 }}</td>
          <td>{{ item.T6 }}</td>
          <td>{{ item.Total }}</td>
          <td>{{ item.Avrage }}</td>
        </tr>
      </table>
    </v-col>
    <v-col cols="6">
      <line-chart
        :chart-data="datacollection"
        :options="datacollection.options"
      ></line-chart>
    </v-col>
  </v-container>
</template>

<script>
import LineChart from "../../components/charts/LineChart.js";
import request from "@/utils/request";
import deviceFilterDropDown from "@/components/common/DeviceFilterDropDown.vue";
import calenderSelect from "@/components/common/CalenderSelect.vue";

export default {
  components: {
    LineChart,
    deviceFilterDropDown,
    calenderSelect,
  },
  data() {
    return {
      loading: false,
      items: [],
      datacollection: [],
    };
  },
  mounted() {
    this.getCableType();
  },
  methods: {
    getCableType() {
      this.loading = true;
      request
        .get("/chart/unique-users")
        .then((response) => {
          var data = response.data.result;
          this.items = data.items;
          var xValues = data.xValues;
          this.datacollection = {
            labels: data.labels,
            datasets: [
              {
                data: [
                  860, 1140, 1060, 1060, 1070, 1110, 1330, 2210, 7830, 2478,
                ],
                borderColor: "red",
                fill: false,
              },
              {
                data: [
                  1600, 1700, 1700, 1900, 2000, 2700, 4000, 5000, 6000, 7000,
                ],
                borderColor: "green",
                fill: false,
              },
              {
                data: [300, 700, 2000, 5000, 6000, 4000, 2000, 1000, 200, 100],
                borderColor: "blue",
                fill: false,
              },
            ],
          };
        })
        .finally((a) => {
          this.loading = false;
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
