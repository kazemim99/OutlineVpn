<template>
  <div class="small">
    <LinuxToturial />
  </div>
</template>

<script>
import request from "@/utils/request";
import LinuxToturial from "@/components/Home/LinuxToturial.vue";

export default {
  components: {
    LinuxToturial,
  },
  data() {
    return {};
  },
};
</script>

<style>
.small {
  max-width: 600px;
  margin: 150px auto;
}
.v-dialog {
  font-family: arial, sans-serif;
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
