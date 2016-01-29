## Set up a scope in Easy Analysis System 

#### Setps:

- Create the Json configration file

Scope Level Configration:
```
{
    "code": "UWP",
    "name": "Universal Windows Platform",
    "forums": ["forum_guid"],
    "fields": [{
        "displayName": "Language",
        "options": [{
            "name": "C#"
        }]
    }, {
        "displayName": "OS",
        "options": [{
            "name": "Windows 8.1"
        }]
    }],
    "topics": [{
        "category": "Application Model",
        "options": [{
            "name": "Microsoft APIs"
        }, {
            "name": "Background Task"
        }]
    }, {
        "category": "Deployment",
        "options": [{
            "name": "Microsoft Store"
        }, {
            "name": "Enterpise"
        }]
    }]
}
```
Forum Level Configration:
```
{
    "forumId": "guid",
    "topics": [{
        "category": "Level One 01",
        "options": [{
            "name": "Level Two 01"
        }, {
            "name": "Level Two 02"
        }]
    }, {
        "category": "Lvel One 01",
        "options": [{
            "name": "Level Two 03"
        }, {
            "name": "Level Two 04"
        }]
    }]
}
```