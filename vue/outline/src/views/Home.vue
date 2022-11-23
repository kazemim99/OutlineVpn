<template>
  <div class="small">
    <v-row>
      <v-col cols="9">

        <div class="grey--text mb-2">
                      {{this.capacity}}
                    </div>
       
      </v-col>
      <v-col cols="2" style="margin-top: 12px">
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
      capacity: 0,
    };
  },
  mounted() {
    this.getCapacity();
  },
  methods: {
    getCapacity() {
      request.get(`/keys/capacity/09123135143`).then((response) => {
        var data = response.data.result;
        this.capacity = data;
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
