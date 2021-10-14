const MongoClient = require('mongodb').MongoClient;
const ObjectID = require('mongodb').ObjectId;
const dbClient = new MongoClient('mongodb+srv://infiniracer:27x3VP7W90MGqYrf@infiniracer.ifeau.mongodb.net/InfiniRacer?retryWrites=true&w=majority', {useNewURLParser: true, useUnifiedTopology: true});

const functions = require("firebase-functions");

// Create attention session
exports.CreateAttentionSession = functions.region('australia-southeast1').https.onRequest(async (req,res) => {
    const name = req.body.name || 'Anonymous';
    const attentionBefore = req.body.attentionBefore;
    const attentionAfter = req.body.attentionAfter;
    const playTime = req.body.playTime;
    try {
        await dbClient.connect();
        const db = dbClient.db('InfiniRacer');
        const attentionSessions = db.collection("AttentionSessions");

        const recordData = {
            name,
            attentionBefore,
            attentionAfter,
            playTime,
        }

        await attentionSessions.insertOne(recordData);

        return res.send('Created attention session successfully!')
    } catch (err) {
        return res.send(err);
    }
});

// Read attention session by id
exports.ReadAttentionSession = functions.region('australia-southeast1').https.onRequest(async (req,res) => {
    const sessionID = req.body.sessionID;
    try {
        await dbClient.connect();
        const db = dbClient.db('InfiniRacer');
        const attentionSessions = db.collection("AttentionSessions");

        const allSessions = await attentionSessions.find().toArray();

        const targetSession = await allSessions.findOne({_id: new ObjectID(sessionID)});

        return res.status(200).send(targetSession);
    } catch (err) {
        return res.send(err);
    }
});

// Get all attention sessions
exports.ReadAllAttentionSession = functions.region('australia-southeast1').https.onRequest(async (req,res) => {
    try {
        await dbClient.connect();
        const db = dbClient.db('InfiniRacer');
        const attentionSessions = db.collection("AttentionSessions");

        const allSessions = await attentionSessions.find().toArray();

        return res.status(200).send({message: 'Found all sessions successfully!', sessions: allSessions});
    } catch (err) {
        return res.send(err);
    }
});