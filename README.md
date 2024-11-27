# CURL 
curl "https://api.github.com/repos/lodash/lodash/contents/PATH" \
    -H "Accept: application/vnd.github+json" \
    --header "Authorization: Bearer $TOKEN" \
    --header "X-GitHub-Api-Version: 2022-11-28"


curl "https://api.github.com/repos/lodash/lodash/git/trees/main?recursive=1" \
    -H "Accept: application/vnd.github+json" \
    --header "Authorization: Bearer $TOKEN" \
    --header "X-GitHub-Api-Version: 2022-11-28"


# Complexity

Algorithm is split is multiple api calls and different parsing stratgies

 * Trees Api : Return a list of F files
 * Lookup each files filtered by a criteria, result in processing J files
 * For each file, converting the base64 content into text and parse it through the entire size S
 * For each char C in the content, filter the letters and add them in a dictionary in constant time.

The global count algorithm is O(J*S*C + (F-J)) which make it linear in total of Chars concat'd in the global buffer.

Sorting the dictionary by Value is O(N*log(N)) in average (N^2 in worst case). But here N is fixed size as 26 in the worst case.

Globally the complexity of this algorith is linear to the total length of content to process.



# Docker images

To run the application ACCESS_TOKEN is optional. Mostly to be used with public repositories.

```
docker build --rm -t infinit .
docker run --rm -e ACCESS_TOKEN=YOUR_TOKEN -e OWNER=lodash -e REPO=lodash infinit
```

Pure dev environment

```
docker build --rm -t infinit.dev -f Dockerfile.dev .
docker run --rm -e ACCESS_TOKEN=YOUR_TOKEN -e OWNER=lodash -e REPO=lodash -v `pwd`:/app -it infinit.dev
```