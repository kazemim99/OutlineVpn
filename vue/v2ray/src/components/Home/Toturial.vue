<template>
  <v-container>
    <v-row justify="center">
      <v-row justify="right">
        <v-col md="12">
          <h1>آموزش استفاده :</h1>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="12">
          <p>
            برای آیفون باید از NapsternetV و برای ویندوز از Netmode Syna باید
            استفاده کنید
          </p>
          <p>برای اندروید از هر دو برنامه میتوان استفاده کرد</p>
          <p>دوستان لینوکسی میتونن از sshuttle استفاده کنن</p>
          <p>sudo apt-get install sshuttle</p>
          <p>sudo sshuttle -p port -r user@remote_host 0.0.0.0/0 --dns</p>
          <p>
            نکته : در صورت درخواست دسترسی توسط برنامه هیچ دسترسی به برنامه ندهید
          </p>
        </v-col>
      </v-row>
      <br />
      <br />
      <v-col col="12" sm="12">
        <h3>آموزش اتصال با NapsternetV</h3>
      </v-col>
      <br />
      <br />

      <v-col md="6" sm="12">
        <v-img
          contain
          src="~@/assets/images//toturial/NapsternetV/1.jpg"
        ></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/2.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/3.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/4.jpg"></v-img>
      </v-col>

      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/5.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/6.jpg"></v-img>
      </v-col>

      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/7.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/8.jpg"></v-img>
      </v-col>

      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/9.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/NapsternetV/10.jpg"></v-img>
      </v-col>

      <br />
      <br />
      <v-col cols="12">
        <h3>آموزش اتصال با Netmode</h3>
      </v-col>
      <br />
      <br />
        <v-col md="6" sm="12">
          <v-img src="~@/assets/images/toturial/1.jpg"></v-img>
        </v-col>
        <v-col md="6" sm="12">
          <v-img src="~@/assets/images/toturial/2.jpg"></v-img>
        </v-col>
        <v-col md="6" sm="12">
          <v-img src="~@/assets/images/toturial/3.jpg"></v-img>
        </v-col>
        <v-col md="6" sm="12">
          <v-img src="~@/assets/images/toturial/4.jpg"></v-img>
        </v-col>

        <v-col md="6" sm="12">
          <v-img src="~@/assets/images/toturial/5.jpg"></v-img>
        </v-col>
        <v-col md="6" sm="12">
          <v-img src="~@/assets/images/toturial/6.jpg"></v-img>
        </v-col>

        <v-col md="6" sm="12">
          <v-img width="70%" src="~@/assets/images/toturial/7.jpg"></v-img>
        </v-col>
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
