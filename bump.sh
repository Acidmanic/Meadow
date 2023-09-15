


function bump_inc(){
    
    release auto inc betainc SourceRoot ../ VersionStandard  beta-semantic
    nugcast
    
}

PROJECTS=$(cat .publishes) 

TAGS="";
TAG="";
NL=$(echo '\n');

for PROJECT in $PROJECTS
do
    (cd $PROJECT && bump_inc)
    TAG=$(git for-each-ref refs/tags --sort=-taggerdate --format "tag:%(refname:strip=2), %(subject)" --count=1);
    TAGS="$TAGS $NL $PROJECT: $TAG"
done


echo $TAGS


