var express = require('express')
var app = express()
var https = require("https");

var query = 'compare the values';
var test;

var mysql = require('mysql');

var connection = mysql.createConnection({
  host: "localhost",
  user: "root",
  password: "admin12345",
  database: "empdata"
});

connection.connect(function(err) {
  if (err) throw err  
});

function insertData() {
  connection.query('SELECT * FROM user', function(err,result) {
    if(err) throw err
    console.log(result);  
  });
}

app.get('/tempdata', function (req, res) {
  res.send('Hello World!22')  
  insertData(); 
})

/*
function insertData(name,id) {
  connection.query('INSERT INTO members (name, id) VALUES (?, ?)', [name,id], function(err,result) {
    if(err) throw err
  });
  http://www.opentechguides.com/askotg/question/41/nodejs-cannot-enqueue-handshake-after-already-enqueuing-a-handshake
}
*/

/*var data = {items: [
    {id: "1", name: "Snatch", type: "crime"},
    {id: "2", name: "Witches of Eastwick", type: "comedy"},
    {id: "3", name: "X-Men", type: "action"},
    {id: "4", name: "Ordinary People", type: "drama"},
    {id: "5", name: "Billy Elliot", type: "drama"},
    {id: "6", name: "Toy Story", type: "children"}
]};


/*const {Wit, log} = require('node-wit');

const client = new Wit({accessToken: 'RDRRC37D34PA6ZRUAWNENFUZZEU5SOYT'});
client.message('get the shopping cart', {})
.then((data) => {
  console.log('Yay, got Wit.ai response: ' + JSON.stringify(data));  
  console.log(data.msg_id);
  console.log(data.entities);
  console.log(data.entities.CartEnt[0].value);
  if(data.entities.CartEnt[0].value == 'cart'){
      console.log('In Cart Ent')
  }
})
.catch(console.error);*/



/*var options = {
    host: 'westus.api.cognitive.microsoft.com',
    path: ''/'luis'/'v2.0'/'apps'/'f0a93ae3-fdd5-4d17-824b-f8aef414c15f?subscription-key=2635884f0d0041c4ab453562efed7460&timezoneOffset=0&verbose=true',
    method: 'GET'
};

var request = https.request(options, function(responce){
    var body = ''
    responce.on("data", function(chunk){
        body += chunk.toString('utf8')
    });
    responce.on("end", function(){
        console.log("Body", body);
    });
});
request.end();

app.get('/tempdata', function (req, res) {
  res.send('Hello World!')
  con.connect(function(err) {
  if (err) throw err;
  con.query("SELECT * FROM user", function (err, result) {
    if (err) throw err;
    console.log(result);
  });
 });
 con.end()  
})

app.get('/tempdata2', function (req, res) {
  res.send('Hello World 222!')

        connection.query("SELECT * FROM user", 
            function(err, results, fields) {
                if (err) return callback(err, null);
                return callback(null, results);
            }
        ); 
  
})




app.get('/', function (req, res) {
  res.send('Hello World!')
  data.items.push(
    {id: "7", name: "Douglas Adams", type: "comedy"}
  );
  console.log(data);
  data.items.splice(1, 1);
  console.log(data);
  console.log(data.items[0].name);
})

app.post('/getData', function (req, res) {
  res.send('World!')
})*/


app.listen(3000, function () {
  console.log('Example app listening on port 3000!')
})