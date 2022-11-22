<template>
 <v-container>
    <deviceFilterDropDown ref="complex" />
    <calenderSelect ref="dateRef" />

    <v-row>
      <v-btn :loading="loading" color="blue darken-1" @click="getProince()"
        >مشاهد</v-btn
      >
    </v-row>
        <v-row>
      <v-col cols="6">
        <table style="margin-top: 80px">
          <tr>
            <th>استان</th>
            <th>تعداد </th>
            <th>درصد</th>
          </tr>
          <tr v-for="item in items" :key="item.message">
            <td>{{ item.province }}</td>
            <td>{{ item.count }}</td>
            <td>{{ item.rate.toFixed(2) }}%</td>
          </tr>
        </table>
      </v-col>
      <v-col cols="6">
        <bar-chart
          :chart-data="datacollection"
          :options="datacollection.options"
        ></bar-chart>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import BarChart from "../../components/charts/BarChart.js";
import request from "@/utils/request";
import deviceFilterDropDown from "@/components/common/DeviceFilterDropDown.vue";
import calenderSelect from "@/components/common/CalenderSelect.vue";
import qs from "qs";

export default {
  components: {
    BarChart,
        deviceFilterDropDown,
    calenderSelect,

  },
  data() {
    return {
      loading:false,
      items: [],
      hasData: false,
      phone: null,
      datacollection: [],
    };
  },

  methods: {
    getProince() {
      this.loading = true;
      request.get("/chart/provinces", {
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
        }).then((response) => {
        var data = response.data.result;
        this.items = data.items;
        this.datacollection = {
          labels: data.yValues,
          datasets: [
            {
              label: "Provinces",
              data: data.xValues,
              backgroundColor: "blue",
            },
          ],
        };
      }).finally(a=>{
         
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
