# -*- coding: utf-8 -*-
from flask import Flask, request
from flask_mysqldb import MySQL
import wikipedia

mysql = MySQL()
app = Flask(__name__)
app.config['MYSQL_DATABASE_USER'] = 'man'
app.config['MYSQL_DATABASE_PASSWORD'] = 'admin1'
app.config['MYSQL_DATABASE_DB'] = 'Data'
app.config['MYSQL_DATABASE_HOST'] = 'localhost'
mysql.init_app(app)

@app.route("/")
def hello():
	return "<h2><u>Fin Assistant</u></h2><h4>For 10q:</h4>Data:http://127.0.0.1:5000/10q/FinData?Datatype=</br>Plots:http://127.0.0.1:5000/10q/FinPlot?Imgtype=<h4>For 10k:</h4>Data:http://127.0.0.1:5000/10k/FinData?Datatype=</br>Plots:http://127.0.0.1:5000/10k/FinPlot?Imgtype=<h4>For 8k:</h4>Data:http://127.0.0.1:5000/8k/FinData?Datatype=</br>Plots:http://127.0.0.1:5000/8k/FinPlot?Imgtype="

@app.route("/Summary")
def Summarize():
	return "As of March 31, 2015 and June 30, 2014, the recorded bases of common and preferred stock that are restricted for more than one year or are not publicly traded were $538 million and $520 million, respectively. These investments are carried at cost and are reviewed quarterly for indicators of other-than-temporary impairment.^As of March 31, 2015, the total notional amounts of fixed-interest rate contracts purchased and sold were $2.1 billion and $2.8 billion, respectively. As of June 30, 2014, the total notional amounts of fixed-interest rate contracts purchased and sold were $1.7 billion and $936 million, respectively.^ On November 6, 2014, we acquired Mojang Synergies AB (“Mojang”), the Swedish video game developer of the Minecraft gaming franchise, for $2.5 billion in cash, net of cash acquired. The addition of Minecraft and its community enhances our gaming portfolio across Windows, Xbox, and other ecosystems and devices outside our own."    

@app.route("/Mail") 
def Authenticate():
	return "Email Sent!"
	
@app.route("/Definition")
def Define():
	term = request.args.get('Def')
	return wikipedia.summary(term,sentences=2)

if __name__ == "__main__":
	app.debug = True
	app.run()