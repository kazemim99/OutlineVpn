<template>
  <div class="small">
    <v-row>

      <v-col cols="6" >
        <div class="grey--text mb-2">ترافیک مصرف شده</div>
      </v-col>
      <v-col cols="6">
        <div class="mb-2">
          {{ this.consumedTraffic }}   گیگا بایت
        </div>
      </v-col>
      
    </v-row>
    <v-row >
      <v-col cols="6" >
        <div class="grey--text mb-2">ترافیک  باقی مانده</div>
      </v-col>
      <v-col cols="6">
        <div class="mb-2">
          {{ this.raminingTraffic }}   گیگا بایت
        </div>
      </v-col>
      
    </v-row>
    <v-row>
      <v-col cols="6" >
        <div class="grey--text mb-2">ترافیک خریداری شده</div>
      </v-col>
      <v-col cols="6">
        <div class="mb-2">
          {{ this.initTraffic }}   گیگا بایت
        </div>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import request from "@/utils/request";
import { UserModule } from "@/store/modules/user";

export default {
  data() {
    return {
      hasData: false,
      phone: null,
      datacollection: null,
      consumedTraffic: 0,
      initTraffic: 0,
      raminingTraffic: 0,
    };
  },
  mounted() {
    this.getConsumedTraffic();
  },
  methods: {
    getConsumedTraffic() {
      request.get(`/keys/consumed-traffic`).then((response) => {
        console.log(response);
        var data = response.data.result;
        this.consumedTraffic = data.consumedTraffic;
        this.initTraffic = data.initTraffic;
        this.raminingTraffic = data.raminingTraffic;
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
