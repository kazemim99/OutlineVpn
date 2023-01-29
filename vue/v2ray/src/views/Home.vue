<template>
  <div>
    <v-card flat>
      <v-container fluid>
        <v-row class="child-flex">
          <div>
            <v-toolbar>
              <v-toolbar-title link to="/login">IranV2Ray</v-toolbar-title>
            </v-toolbar>
          </div>

          <div style="flex-basis: 20%">
            <v-toolbar dark>
              <v-spacer></v-spacer>

              <v-btn small to="/login" link> ورود /ثبت نام </v-btn>
            </v-toolbar>
          </div>
        </v-row>
      </v-container>
    </v-card>
    <v-container>
      <div>
        <v-row>
          <div class="text-h3">فروش فیلتر شکن</div>
        </v-row>
        <br />
        <br />
        <v-row>
           جهت دریافت فیلتر شکن رایگان به صفحه 
          <a small class="mb-0" color="primary" text href="/login"> ثبت نام </a>
           بروید 
        </v-row>
      </div>
    </v-container>
    <br />
    <v-container>
      <v-row dense>
        <v-col cols="4">
          <v-card class="mx-auto" max-width="344">
            <v-img src="/phone.jpg" height="200px"></v-img>

            <v-card-title> فیلتر شکن اندروید و ایفون </v-card-title>

            <v-card-subtitle>پر سرعت بدون قطعی</v-card-subtitle>

            <v-card-actions>
              <v-spacer></v-spacer>
            </v-card-actions>

            <v-expand-transition>
              <div>
                <v-divider></v-divider>

                <v-card-text>
                  با استفاده از نرم افزار NapsternetV و دریافت ثبت نام و دریافت
                  تنظیمات از پنل کاربری خود بدون هیچ محدودیتی میتوانید هر
                  اپلیکیشنی را باز کنید و فایلهای خود را با سرعت بالا آپلود کرده
                  یا دانلود کنید
                </v-card-text>
              </div>
            </v-expand-transition>
          </v-card>
        </v-col>
        <v-col cols="4">
          <v-card class="mx-auto" max-width="344">
            <v-img src="/windows.jpg" height="200px"></v-img>

            <v-card-title> فیلتر شکن ویندوز </v-card-title>

            <v-card-subtitle>نصب ساده و سریع</v-card-subtitle>

            <v-card-actions>
              <v-spacer></v-spacer>
            </v-card-actions>

            <v-expand-transition>
              <div>
                <v-divider></v-divider>

                <v-card-text>
                  با استفاده از نرم افزار NetMod Syna و انجام چند کانفیگ ساده بر
                  روی سیستم عامل ویندوز و بدون ایجاد تنظیمات اضافه رو شبکه خود
                  به راحتی از فیلترینگ میتوانید عبور کنید
                </v-card-text>
              </div>
            </v-expand-transition>
          </v-card>
        </v-col>
        <v-col cols="4">
          <v-card class="mx-auto" max-width="344">
            <v-img src="/linux.jpg" height="200px"></v-img>

            <v-card-title> فیلتر شکن لینوکس </v-card-title>

            <v-card-subtitle> کانفیگ ساده </v-card-subtitle>

            <v-card-actions>
              <v-spacer></v-spacer>
            </v-card-actions>

            <v-expand-transition>
              <div>
                <v-divider></v-divider>

                <v-card-text>
                  با استفاده از چند command ساده براحتی و باسرعت بسیار بالا و
                  بدون قطعی میتوانید از طریق سیستم عامل لینوکس به سرورهای ما
                  متصل شده و ازادانه از اینترنت آزاد استفاده کنید
                </v-card-text>
              </div>
            </v-expand-transition>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>
<script>
import request from "@/utils/request";

export default {
  components: {},
  data() {
    return {};
  },
};
</script>

