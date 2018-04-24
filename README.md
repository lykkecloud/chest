# Chest #

Lykke Web API to store and retrieve key value data
It stores any key value pairs against a unique key

### How to configure

### It runs on port 5011

### It exposes following end-points

## GET /api/metadata/{key}

## POST /api/metadata
Content-Type: application/json

BODY

{"key": "my-unique-key", "data": {["key_name", "value"]}}