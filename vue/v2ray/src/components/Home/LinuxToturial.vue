<template>
  <v-container>
    <v-row justify="center">
      <v-row justify="right">
        <v-col md="12">
          <h1>آموزش استفاده :</h1>
          <h4>
            ابتدا برای اتصال از
            <v-btn link to="/dashboard/buy-vpn">صفحه اصلی</v-btn>
            اطلاعات اتصال را دریافت کنید
          </h4>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="12">
          <p>دوستان لینوکسی میتونن از sshuttle استفاده کنن</p>
          <p>sudo apt-get install sshuttle</p>
          <p>sudo sshuttle -p port -r user@remote_host 0.0.0.0/0 --dns</p>
          <p>
            نکته : در صورت درخواست دسترسی توسط برنامه هیچ دسترسی به برنامه ندهید
          </p>
        </v-col>
      </v-row>
    </v-row>
  </v-container>
</template>

<style scoped>
.v-card--reveal {
  align-items: center;
  bottom: 0;
  justify-content: center;
  opacity: 0.5;
  position: absolute;
  width: 100%;
}

.card-form-img {
  padding: 0px !important;
}

.icon-btn-modal {
  position: absolute;
  font-size: 18px !important;
  color: #fff !important;
  padding: 8px;
  border-radius: 50%;
}

.icon-btn-modal:hover {
  cursor: pointer;
}

.icon-btn-upload {
  position: absolute;
  left: 60%;
  bottom: 33%;
  color: #fff !important;
  /*padding: 8px;*/
  border-radius: 50%;
  /*background: #35495E !important;*/
  text-align: center;
  display: flex;
  margin: auto;
  justify-content: center;
  align-items: center;
  /*height: 20px !important;*/
  /*width: 20px !important;*/
}

.v-icon {
  color: #fff !important;
  font-size: 18px !important;
  text-align: center;
  background: #35495e !important;
}

.logo-title {
  text-align: center;
  display: flex;
  justify-content: center;
  margin-bottom: 15px;
}
</style>
