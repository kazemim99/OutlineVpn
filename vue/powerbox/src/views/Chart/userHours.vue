<template>
  <v-container>
    <deviceFilterDropDown ref="complex" />
    <calenderSelect ref="dateRef" />

    <v-row>
      <v-btn :loading="loading" color="blue darken-1" @click="userHours()"
        >مشاهد</v-btn
      >
    </v-row>
    <v-row>
      <v-col cols="6">
        <table style="margin-top: 80px">
          <tr>
            <th>Hour</th>
            <th>% of</th>
          </tr>
          <tr v-for="item in items" :key="item.hour">
            <td>{{ item.hour }}</td>
            <td>{{ item.rate.toFixed(2) }} %</td>
          </tr>
        </table>
      </v-col>
      <v-col cols="6">
        <doughnut-chart
          :chart-data="datacollection"
          :options="datacollection.options"
        ></doughnut-chart>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import DoughnutChart from "../../components/charts/LineChart.js";
import request from "@/utils/request";
import deviceFilterDropDown from "@/components/common/DeviceFilterDropDown.vue";
import calenderSelect from "@/components/common/CalenderSelect.vue";
import qs from "qs";

export default {
  components: {
    DoughnutChart,
    deviceFilterDropDown,
    calenderSelect,
  },
  data() {
    return {
      loading: false,
      datacollection: {},
      items: [],
    };
  },
  methods: {
    userHours() {
      this.loading = true;
      request
        .get("/chart/user-hours", {
          params: {
            from: this.$refs.dateRef.from,
            to: this.$refs.dateRef.to,
            complexId: this.$refs.complex.selectedSubComplexId
              ? this.$refs.complex.selectedSubComplexId
              : this.$refs.complex.selectedComplexId,
            deviceId: this.$refs.complex.deviceId,
          },
          paramsSerializer: (params) => {
            return qs.stringify(params, { arrayFormat: "repeat" });
          },
        })
        .then((response) => {
          var data = response.data.result;
          this.items = data.items;
          this.datacollection = {
            labels: data.yValues,
            datasets: [
              {
                label: "rate",
                data: data.xValues,
                backgroundColor: "rgba(0,0,0,0.1)",
                borderColor: "rgba(55,220,88,1.0)",
              },
            ],
            // options: {
            //   responsive: true,
            //   maintainAspectRatio: true,
            //   plugins: {
            //     datalabels: {
            //       formatter: (value, ctx) => {
            //         let sum = 0;
            //         let dataArr = ctx.chart.data.datasets[0].data;
            //         dataArr.map((data) => {
            //           sum += data;
            //         });
            //         let percentage = ((value * 100) / sum).toFixed(2) + "%";
            //         return percentage;
            //       },
            //       color: "#fff",
            //     },
            //   },
            // },
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
