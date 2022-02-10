/*
 * GET home page.
 */
import fetch = require('node-fetch')
import express = require('express');
import { TextDecoder } from 'util';
const router = express.Router();

router.get('/', async (req: express.Request, res: express.Response) => {

    const response = await fetch("http:netcoreapplication");

    if (!response.ok) { /* Handle */ }

    let pageRes;
    // If you care about a response:
    if (response.body !== null) {
        // body is ReadableStream<Uint8Array>
        // parse as needed, e.g. reading directly, or
        const asString = new TextDecoder("utf-8").decode(response.body);
        // and further:
        pageRes = JSON.parse(asString);  // implicitly 'any', make sure to verify type on runtime.
        console.info(pageRes)
    }

    res.render('index', { title: 'Express', text: pageRes});
});

export default router;