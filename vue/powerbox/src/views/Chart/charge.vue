<template>
  <v-container>
    <deviceFilterDropDown ref="complex" />
    <calenderSelect ref="dateRef" />

    <v-row>
      <v-btn :loading="loading" color="blue darken-1" @click="getCableType()"
        >مشاهد</v-btn
      >
    </v-row>
    <v-row>
      <v-col cols="6">
        <table style="margin-top: 80px">
          <tr>
            <th>Android</th>
            <th>Apple</th>
          </tr>
          <tr>
            <td>{{ cableType.androidRate }}%</td>
            <td>{{ cableType.iosRate }}%</td>
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
import DoughnutChart from "../../components/charts/DoughnutChart.js";
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
      hasData: false,
      phone: null,
      datacollection: {},
      cableType: {},
    };
  },

  methods: {
    getCableType() {
      this.loading = true;
      request
        .get(`/chart/cable-types?`, {
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
          this.cableType = data;
          this.datacollection = {
            labels: data.labels,
            datasets: [
              {
                label: "نوع شارژر",
                data: data.cableTypeRates,
                backgroundColor: ["rgb(255, 99, 132)", "rgb(54, 162, 235)"],
              },
            ],
          };
        }).finally(a=>{
                    this.loading = false;

        });
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
