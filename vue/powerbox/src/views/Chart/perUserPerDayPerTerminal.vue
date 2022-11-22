<template>
  <v-container>
    <h3>پراکندگی روزهای شارژ به ازای هر روز و ترمینال و کاربران</h3>
    <deviceFilterDropDown ref="complex" />
    <calenderSelect ref="dateRef" />

    <v-row>
      <v-btn :loading="loading" color="blue darken-1" @click="getRates()"
        >مشاهد</v-btn
      >
    </v-row>
    <v-row>
      <!-- <v-col cols="6">
        <table style="margin-top: 80px">
            <th>date</th>
          <th v-for="item in terminals" :key="item">
            {{ item }}
          </th>
        

          <tr v-for="(item) in labels" :key="item">
            <td v-for="ter in terminals" :key="ter">
              {{
                items[index].data.reduce(function (a, b) {
                  return a + b;
                }, 0)
              }}
            </td>
            <td>{{ item }}</td>
          </tr>
        </table>
      </v-col> -->
      <v-col cols="12">
        <line-chart
          :chart-data="datacollection"
          :options="datacollection.options"
        ></line-chart>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import LineChart from "../../components/charts/LineChart.js";
import request from "@/utils/request";
import deviceFilterDropDown from "@/components/common/DeviceFilterDropDown.vue";
import calenderSelect from "@/components/common/CalenderSelect.vue";
import qs from "qs";

export default {
  components: {
    LineChart,
    deviceFilterDropDown,
    calenderSelect,
  },
  data() {
    return {
      loading: false,
      labels: [],
      terminals: [],
      hasData: false,
      phone: null,
      datacollection: {},
      items: [],
    };
  },

  methods: {
    getRates() {
      this.loading = true;
      request
        .get("/chart/per-user-per-day-per-terminal", {
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
          this.labels = data.labels;
          this.items = data.datasets;
          this.terminals = data.terminals;
          this.datacollection = {
            labels: data.labels,
            datasets: data.datasets,
            options: {
              legend: { display: false },
            },
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
