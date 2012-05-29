from encodings import hex_codec
import json

def replace_value(value, independent_variable, prev_independent_variable, data_set_name):
    if value == "[x]":
        return independent_variable
    if value == "[n]":
        return data_set_name
    if value == "[x-1]":
        if prev_independent_variable == "-inf":
            return -float('inf')
        return prev_independent_variable
    return value

def condition_match(item, condition, independent_variable, prev_independent_variable, data_set_name):
    if condition.type == 0:
        return True

    test_key = replace_value(condition.key, independent_variable, prev_independent_variable, data_set_name)

    if condition.type == 1 or condition.type == 2 or condition.type == 3:
        expected_value = replace_value(condition.value, independent_variable, prev_independent_variable, data_set_name)

    if condition.type ==2 or condition.type == 5:
        actualIndex = int(replace_value(condition.index, independent_variable, prev_independent_variable, data_set_name))
    if condition.type ==3 or condition.type == 6:
        actualDictKey = replace_value(condition.dictKey, independent_variable, prev_independent_variable, data_set_name)

    if condition.type == 1 or condition.type == 4:
        actual_value = item[test_key]
    if condition.type == 2 or condition.type == 5:
        actual_value = item[test_key][actualIndex]
    if condition.type == 3 or condition.type ==6:
        actual_value = item[test_key][actualDictKey]

    if condition.type == 4 or condition.type == 5 or condition.type ==6:
        actualMin = float(replace_value(condition.min, independent_variable, prev_independent_variable, data_set_name))
        actualMax = float(replace_value(condition.max, independent_variable, prev_independent_variable, data_set_name))

    if condition.type == 1 or condition.type == 2 or condition.type == 3:
        return str(actual_value) == str(expected_value)
    else:
        return float(actual_value) > float(actualMin) and float(actual_value) <= float(actualMax)


def extract_value(item, target, independent_variable, prev_independent_variable, data_set_name):
    try:
        actual_key = replace_value(target.key, independent_variable, prev_independent_variable, data_set_name)

        if target.type == 0:
            return item[actual_key]
        elif target.type == 1:
            actual_index = replace_value(target.index, independent_variable, prev_independent_variable, data_set_name)
            return item[actual_key][int(actual_index)]
        else:
            actual_dictKey = replace_value(target.dictKey, independent_variable, prev_independent_variable, data_set_name)
            return item[actual_key][actual_dictKey]
    except:
        return "Error"




def percent_over_items(source_json, item_name, primary_condition,
                        secondary_condition, independent_variable, prev_independent_variable, data_set_name ):
    source_data = json.loads(source_json)
    top = 0
    for game in source_data:
        for item in game[item_name]:
            match = True
            for cond in primary_condition:
                if condition_match(item, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                    match = False
            for cond in secondary_condition:
                if condition_match(item, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                    match = False
            if match == True:
                top=top+1
    total = 0
    for game in source_data:
        for item in game[item_name]:
            match = True
            for cond in primary_condition:
                if condition_match(item, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                    match = False
            if match == True:
                total=total+1
    if total==0:
        return 0
    return int(top*100/total)

def percent_over_games(source_json, primary_condition,
                        secondary_condition, independent_variable, prev_independent_variable, data_set_name ):
    source_data = json.loads(source_json)
    top = 0
    for game in source_data:
        match = True
        for cond in primary_condition:
            if condition_match(game, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                match = False
        for cond in secondary_condition:
            if condition_match(game, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                match = False
        if match == True:
            top=top+1
    total = 0
    for game in source_data:
        match = True
        for cond in primary_condition:
            if condition_match(game, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                match = False
        if match == True:
            total=total+1
    if total==0:
        return 0
    return int(top*100/total)

def count_over_items(source_json, item_name, primary_condition,
                        independent_variable, prev_independent_variable, data_set_name ):
    source_data = json.loads(source_json)

    total = 0
    for game in source_data:
        for item in game[item_name]:
            match = True
            for cond in primary_condition:
                if condition_match(item, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                    match = False
            if match == True:
                total=total+1
    return int(total)

def count_over_games(source_json, primary_condition,
                        independent_variable, data_set_name ):
    source_data = json.loads(source_json)

    total=0
    for game in source_data:
        match = True
        for cond in primary_condition:
            if condition_match(game, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                match = False
        if match == True:
            total=total+1

    return int(total)

def average_over_items(source_json, item_name, target, primary_condition,
                        independent_variable, prev_independent_variable, data_set_name ):
    source_data = json.loads(source_json)

    total = 0
    count = 0
    for game in source_data:
        for item in game[item_name]:
            match = True
            for cond in primary_condition:
                if condition_match(item, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                    match = False
            if match == True:
                v = extract_value(item, target, independent_variable, prev_independent_variable, data_set_name)
                if v!="Error":
                    total += v
                    count+=1
    if count==0:
        return 0
    return 1.0*total/count

def average_over_games(source_json, target, primary_condition,
                        independent_variable, prev_independent_variable, data_set_name ):
    source_data = json.loads(source_json)

    total = 0
    count = 0
    for game in source_data:
        match = True
        for cond in primary_condition:
            if condition_match(game, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                match = False
        if match == True:
            v = extract_value(game, target, independent_variable, prev_independent_variable, data_set_name)
            if v!="Error":
                total += v
                count+=1
    if count==0:
        return 0
    return 1.0*total/count

def sum_over_items(source_json, item_name, target, primary_condition,
                        independent_variable, data_set_name ):
    source_data = json.loads(source_json)

    total = 0
    for game in source_data:
        for item in game[item_name]:
            match = True
            for cond in primary_condition:
                if condition_match(item, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                    match = False
            if match == True:
                v = extract_value(item, target, independent_variable, prev_independent_variable, data_set_name)
                if v!="Error":
                    total += v
    return total

def sum_over_games(source_json, target, primary_condition,
                        independent_variable, prev_independent_variable, data_set_name ):
    source_data = json.loads(source_json)

    total = 0
    for game in source_data:
        match = True
        for cond in primary_condition:
            if condition_match(game, cond, independent_variable, prev_independent_variable,data_set_name) == False:
                match = False
        if match == True:
            v = extract_value(game, target, independent_variable, prev_independent_variable, data_set_name)
            if v!="Error":
                total += v
    return total