const SYSTEM_CODE = 3;
export const environment = {
  production: true,
  systemCode: SYSTEM_CODE,
  apiUrlEC: '/api/', //system
  api_login_Url: 'http://10.4.0.86:1005/api/', //login , CRUD account
  hub: '/ec-hub',
  scalingHub: 'http://localhost:5001/ec-hub',
  apiLocal: 'http://localhost:5003/api/',
  rpmHub: 'http://localhost:5003/rpm-hub',
  scalingHubLocal: 'http://localhost:5001/scalingHub',
  _mqtt: {
    server: 'mqtt.myweb.com',
    protocol: "wss",
    port: 1883
  },
  get mqtt() {
    return this._mqtt;
  },
  set mqtt(value) {
    this._mqtt = value;
  },
};
