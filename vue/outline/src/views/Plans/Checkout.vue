<template>
    <v-container fluid>
      <v-row dense>
       
      </v-row>
    </v-container>
  </template>
  
  <script>
  import request from "@/utils/request";
  import Vue from "vue";
  
  
  export default {
    data: () => ({
      cards: [],
    }),
    mounted() {
      this.getConsumedTraffic();
    },
    
    methods: {
      formatPrice(value) {
          let val = (value/1).toFixed().replace('.', ',')
          return val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
      },
      getConsumedTraffic() {
        request
          .get(`plan/plans`)
          .then((response) => {
            let data = response.data.result;
            this.cards = data.result;
          })
      },
      reserve(id) {
        request
          .post(`/keys`, this.input)
          .then(() => {
            alert("success");
          })
          .finally(() => {
            console.log("finaly");
          });
      },
    },
  };
  
  //  createKey(id){
  //     Vue.swal({
  //       title: "ایا مطمئن  هستید",
  //       icon: "warning",
  //       showCancelButton: true,
  //       confirmButtonColor: "#3085d6",
  //       cancelButtonColor: "#d33",
  //       confirmButtonText: "بله ,حذف شود",
  //       cancelButtonText: "انصراف",
  //     }).then((result) => {
  //       if (result.isConfirmed) {
  //         request
  //           .delete(`/support/${id}`)
  //           .then(() => {
  //             Vue.swal("", "پشتیبان با موفقیت حذف گردید", "success");
  //             this.getSupportes();
  //           })
  //           .finally(() => {
  //             this.uploadLoading = false;
  //           });
  //       }
  //     });
  //   },
  </script>
  
  
  