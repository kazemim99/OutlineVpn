module.exports = {
  devServer: {
    headers: { "Access-Control-Allow-Origin": "*" },
    proxy: "https://localhost:5001/",
  },
  outputDir: "C:/Users/Mostafa/Documents/Projects/powerbox/PoweBox.Api/wwwroot",
  transpileDependencies: ["vuetify"],
  css: {
    loaderOptions: {
      scss: {
        prependData: `@import "@/assets/sass/_variables.scss"; @import "@/assets/sass/_mixins.scss";`,
      },
    },
  },
};